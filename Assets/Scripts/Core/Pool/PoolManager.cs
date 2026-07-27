using System;
using System.Collections.Generic;
using Core.DI;
using Core.Global;
using Core.Log;
using Core.Mono;
using Core.Systems.Memorys;
using UnityEngine;
using Logger = Core.Log.Logger;
using Object = UnityEngine.Object;

namespace Core.Pool
{
    /// <summary>
    /// 缓存池管理器
    /// </summary>
    public class PoolManager : IPoolManager, IMemoryListener
    {
        /// <summary>
        /// 释放策略
        /// </summary>
        public enum EReleaseStrategy
        {
            /// <summary>
            /// 裁剪所有池子
            /// </summary>
            Trim,
            /// <summary>
            /// 最不常用的先释放，根据使用时间来决定，越早使用越先释放
            /// </summary>
            LRU,
        }
        
        private readonly Dictionary<string, IPool> _pools = new();
        
        private readonly LinkedList<string> _lruPoolIds = new();

        // 缓存池根对象
        private GameObject _poolRootObj;
        // 是否开启对象池布局
        private readonly bool _isOpenLayout;
        // 活跃时间阈值，大于该数值为惰性，小于为活跃
        private readonly float activeTimeThreshold;
        // 池子统一最小阈值
        private readonly int poolMinSize;
        // 池子统一最大阈值
        private readonly int poolMaxSize;
        
        private PoolManager()
        {
            _isOpenLayout = GlobalSettings.Instance.isOpenLayout;
            activeTimeThreshold = GlobalSettings.Instance.activeTimeThreshold;
            poolMinSize = GlobalSettings.Instance.poolMinSize;
            poolMaxSize = GlobalSettings.Instance.poolMaxSize;
        }

        public T Get<T>(string key) where T : Object
        {
            // 存在该对象就取出来使用
            if (_pools.ContainsKey(key) && _pools[key].InactiveCount > 0)
            {
                var pool = _pools[key];
                InsertFirst(pool.PoolId);
                var cacheObj = ((IPool<T>)pool).Get();
                return cacheObj;
            }
            return null;
        }

        public void PushObj<T>(T obj) where T : Object
        {
            if (!obj)
            {
                Logger.LogError(ELogTags.Pool, $"{nameof(PoolManager)}: The object to be cached is null.");
                return;
            }
            
            // 第一次缓存对象时初始化缓存池存储结构
            if (!_poolRootObj && _isOpenLayout)
            {
                _poolRootObj = new GameObject("Pool");
                Object.DontDestroyOnLoad(_poolRootObj);
            }
            
            // 已经存储过了就可以直接往容器中存储对象
            if (_pools.TryGetValue(obj.name, out var monoPool))
            {
                InsertFirst(monoPool.PoolId);
                ((IPool<T>)monoPool).Push(obj);
            }
            else
            {
                // 第一次存储要创建存储容器
                var newObjectPool = new ObjectPool<T>(_poolRootObj, obj.name, _isOpenLayout, activeTimeThreshold, poolMinSize, poolMaxSize);
                InsertFirst(newObjectPool.PoolId);
                newObjectPool.Push(obj);
                // 缓存字典
                _pools.Add(obj.name, newObjectPool);
            }
        }

        /// <summary>
        /// 移动到链表头
        /// </summary>
        /// <param name="poolId"></param>
        private void InsertFirst(string poolId)
        {
            if (_lruPoolIds.Contains(poolId))
            {
                _lruPoolIds.Remove(poolId);
            }

            _lruPoolIds.AddFirst(poolId);
        }
        
        public T GetData<T>() where T : class, IPoolData
        {
            // 自定义获取名称，与存储名称一致
            var dataName = $"{typeof(T).FullName}";
            InsertFirst(dataName);
            if (!_pools.TryGetValue(dataName, out var dataPool) || dataPool is not DataPool<T> poolData || poolData.InactiveCount <= 0)
            {
                return DIContainer.Create<T>();
            }
            
            var data = poolData.Get();
            // 注入内容
            DIContainer.InjectIntoInstance(data);
            return data;
        }

        public void PushData<T>(T data) where T : class, IPoolData
        {
            // 自定义缓存名称，与获取名称一致
            var dataName = $"{typeof(T).FullName}";
            if (_pools.TryGetValue(dataName, out var basePoolData))
            {
                (basePoolData as DataPool<T>)?.Push(data);
            }
            else
            {
                var poolData = new DataPool<T>(activeTimeThreshold, poolMinSize, poolMaxSize);
                poolData.Push(data);
                _pools.Add(dataName, poolData);
            }
            
            InsertFirst(dataName);
        }

        /// <summary>
        /// 获取指定资源缓存的数量
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public int GetUnUsedCount(string assetName)
        {
            return _pools.TryGetValue(assetName, out var obj) ? obj.InactiveCount : 0;
        }
        
        public int ReleaseCache(string key)
        {
            if (!_pools.TryGetValue(key, out var poolObj))
            {
                return 0;
            }

            var count = poolObj.InactiveCount;
            poolObj.ClearAll();
            _pools.Remove(key);
            return count;
        }
        
        public void ClearAll()
        {
            foreach (var poolObj in _pools.Values)
            {
                poolObj.ClearAll();
            }
            
            _pools.Clear();
            EngineUtility.Destroy(_poolRootObj);
            _poolRootObj = null;
            GC.Collect();
        }

        /// <summary>
        /// 强制释放内存，可指定释放的选择策略
        /// </summary>
        /// <param name="disposalStrategy"></param>
        /// <param name="executeCount">执行次数，即释放的池子数量，仅当EReleaseStrategy为LRU时使用</param>
        public void ReleaseCache(EReleaseStrategy disposalStrategy = EReleaseStrategy.Trim, ushort executeCount = 5)
        {
            if(_pools.Count == 0)
                return;

            switch (disposalStrategy)
            {
                case EReleaseStrategy.Trim:
                    // 裁剪所有池子缓存
                    foreach (var pool in _pools.Values)
                    {
                        pool.Trim();
                    }
                    break;
                case EReleaseStrategy.LRU:
                    var cur = _lruPoolIds.Last;
                    while (cur != null && executeCount > 0)
                    {
                        var pre = cur.Previous;
                        var releaseId = cur.Value;
                        var releasePool = _pools[releaseId];
                        if (releasePool.IsLazy)
                        {
                            _lruPoolIds.RemoveLast();
                            releasePool.ClearAll();
                            _pools.Remove(releaseId);
                            --executeCount;
                        }

                        cur = pre;
                    }

                    var dels = new List<string>(_pools.Count);
                    foreach (var (id, pool) in _pools)
                    {
                        if (pool.ActiveCount == 0)
                        {
                            dels.Add(id);
                        }
                    }
                    
                    foreach (var del in dels)
                    {
                        _pools.Remove(del);
                        _lruPoolIds.Remove(del);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(disposalStrategy), disposalStrategy, null);
            }
        }

        public void OnReport(MemoryData memoryData)
        {
            switch (memoryData.level)
            {
                case EMemoryOccupationLevel.Normal:
                    return;
                case EMemoryOccupationLevel.Warning:
                    ReleaseCache();
                    break;
                case EMemoryOccupationLevel.Critical:
                    ReleaseCache(EReleaseStrategy.LRU);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
