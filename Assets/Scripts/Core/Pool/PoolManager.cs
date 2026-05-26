using System;
using System.Collections.Generic;
using Core.DI;
using Core.Global;
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
        /// 销毁策略
        /// </summary>
        public enum EDisposalStrategy
        {
            /// <summary>
            /// 按优先级销毁，根据EObjectType优先级，低的先释放
            /// </summary>
            Priority,
            /// <summary>
            /// 最不常用的先释放，根据使用时间来决定，越早使用越先释放
            /// </summary>
            LRU,
        }
        
        // 存储继承Mono对象，便于查找
        private readonly Dictionary<string, ObjectPool> _objectPools = new();
        // 活跃列表
        private readonly List<ObjectPool> _actives = new();
        // 惰性列表
        private readonly List<ObjectPool> _lazies = new();
        // 存储不继承Mono对象
        private readonly Dictionary<string, BasePoolData> _poolDataDic = new();
        // 缓存池根对象
        private GameObject _poolRootObj;
        // 是否开启对象池布局
        private readonly bool _isOpenLayout;
        /// 临界活跃数，高于该数值则放入活跃队列，小于则放入惰性队列
        private const int CriticalActiveCount = 2;
        /// 默认释放次数
        private const byte DefaultReleaseCount = 5;
        
        private PoolManager()
        {
            _isOpenLayout = GlobalSettings.Instance.isOpenLayout;
        }

        public T Get<T>(string key) where T : Object
        {
            // 存在该对象就取出来使用
            if (_objectPools.ContainsKey(key) && _objectPools[key].UnUsedCount > 0)
            {
                var pool = _objectPools[key];
                var cacheObj = pool?.Get<T>();
                // 只有取出的对象不为空，才去更新子池状态
                if (cacheObj)
                    UpdatePoolState(pool);
                return cacheObj;
            }
            return null;
        }

        public void PushObj(Object obj)
        {
            if (!obj)
            {
                Logger.LogError($"{nameof(PoolManager)}: The object to be cached is null.");
                return;
            }
            
            // 第一次缓存对象时初始化缓存池存储结构
            if (!_poolRootObj && _isOpenLayout)
            {
                _poolRootObj = new GameObject("Pool");
            }
            
            // 已经存储过了就可以直接往容器中存储对象
            if (_objectPools.TryGetValue(obj.name, out var objectPool))
            {
                objectPool?.Push(obj);
                // 更新该对象子池状态
                UpdatePoolState(objectPool);
            }
            else
            {
                // 第一次存储要创建存储容器
                objectPool = new ObjectPool(_poolRootObj, obj.name, obj.GetType(), _isOpenLayout, PoolUtil.ConvertFrom(obj));
                objectPool.Push(obj);
                // 缓存字典
                _objectPools.Add(obj.name, objectPool);
                // 默认放入惰性队列
                _lazies.Add(objectPool);
            }
        }
        
        public T GetData<T>() where T : class, IPoolData
        {
            // 自定义获取名称，与存储名称一致
            var dataName = $"{typeof(T).FullName}";
            if (!_poolDataDic.TryGetValue(dataName, out var basePoolData) || basePoolData is not PoolData<T> poolData || poolData.UnUsedCount <= 0)
            {
                return DIContainer.Create<T>();
            }

            var data = poolData.Get();
            // 注入内容
            DIContainer.InjectIntoInstance(data);
            return data;
        }

        public void PushData<T>(T data) where T : class, IPoolData, new()
        {
            // 自定义缓存名称，与获取名称一致
            var dataName = $"{typeof(T).FullName}";
            if (_poolDataDic.TryGetValue(dataName, out var basePoolData))
            {
                (basePoolData as PoolData<T>)?.Push(data);
            }
            else
            {
                var poolData = new PoolData<T>();
                poolData.Push(data);
                _poolDataDic.Add(dataName, poolData);
            }
        }

        /// <summary>
        /// 获取指定资源缓存的数量
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public int GetUnUsedCount(string assetName)
        {
            return _objectPools.TryGetValue(assetName, out var obj) ? obj.UnUsedCount : 0;
        }
        
        public int ClearCache(string key)
        {
            if (!_objectPools.TryGetValue(key, out var poolObj))
            {
                return 0;
            }

            var count = poolObj.UnUsedCount;
            poolObj.Clear();
            _objectPools.Remove(key);
            _lazies.Remove(poolObj);
            _actives.Remove(poolObj);
            return count;
        }
        
        public void ClearAll()
        {
            foreach (var poolObj in _objectPools.Values)
            {
                poolObj.Clear();
            }

            _poolRootObj = null;
            _objectPools.Clear();
            _poolDataDic.Clear();
            _actives.Clear();
            _lazies.Clear();
            GC.Collect();
        }

        /// <summary>
        /// 强制释放内存，可指定释放的选择策略
        /// </summary>
        /// <param name="disposalStrategy"></param>
        /// <param name="executeCount">执行次数，即释放的池子数量</param>
        public void ReleaseCache(EDisposalStrategy disposalStrategy = EDisposalStrategy.Priority, ushort executeCount = 1)
        {
            while (executeCount > 0 && _objectPools.Count > 0)
            {
                ObjectPool destroyPool = null;
                switch (disposalStrategy)
                {
                    case EDisposalStrategy.Priority:
                    {
                        // 先从惰性列表中释放
                        // 默认升序
                        _lazies.Sort((x, y) => x.ObjectType.CompareTo(y.ObjectType));
                        if (_lazies.Count > 0)
                        {
                            destroyPool = _lazies[0];
                            _lazies.RemoveAt(0);
                        }
                        
                        if(destroyPool != null) break;
                        // 再考虑从活跃列表中释放
                        _actives.Sort((x, y) => x.ObjectType.CompareTo(y.ObjectType));
                        if (_actives.Count > 0)
                        {
                            destroyPool = _actives[0];
                            _actives.RemoveAt(0);
                        }
                        break;
                    }
                    case EDisposalStrategy.LRU:
                    {
                        // 先从惰性列表中释放
                        // 根据LRU策略，释放部分内存
                        foreach (var objectPool in _lazies)
                        {
                            // 在惰性列表中寻找最不常用的池子
                            if (destroyPool == null || destroyPool.LastUsedTime > objectPool.LastUsedTime)
                            {
                                destroyPool = objectPool;
                            }
                        }
                        // 从列表中移除
                        _lazies.Remove(destroyPool);
                        if (destroyPool != null) break;
                        
                        // 再考虑从活跃列表中释放
                        foreach (var objectPool in _actives)
                        {
                            // 在活跃列表中寻找最不常用的池子
                            if (destroyPool == null || destroyPool.LastUsedTime > objectPool.LastUsedTime)
                            {
                                destroyPool = objectPool;
                            }
                        }
                        // 从列表中移除
                        _actives.Remove(destroyPool);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(disposalStrategy), disposalStrategy, null);
                }

                // 清空池子缓存
                destroyPool?.Clear();
                // 从缓存中移除
                _objectPools.Remove(destroyPool?.PoolId);
                --executeCount;
            }
        }

        /// <summary>
        /// 更新当前池子活跃状态
        /// </summary>
        /// <param name="pool"></param>
        private void UpdatePoolState(ObjectPool pool)
        {
            // 处理已经在活跃列表中的池子
            if (_actives.Contains(pool) && pool.UsedCount < CriticalActiveCount)
            {
                // 先从活跃列表中移除
                _actives.Remove(pool);
                // 放入在惰性列表
                _lazies.Add(pool);
            }
            // 处理已经在惰性列表中的池子
            else if(_lazies.Contains(pool) && pool.UsedCount >= CriticalActiveCount)
            {
                // 先从惰性列表中移除
                _lazies.Remove(pool);
                // 放入活跃列表
                _actives.Add(pool);
            }
        }

        public void OnReport()
        {
            ReleaseCache(executeCount: DefaultReleaseCount);
        }
    }
}
