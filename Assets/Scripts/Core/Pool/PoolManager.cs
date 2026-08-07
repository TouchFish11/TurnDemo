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
            /// 仅裁剪所有池子
            /// </summary>
            Trim,
            /// <summary>
            /// 混合释放。对于指定的释放次数，先尝试释放未使用的惰性池，再尝试释放“未使用”的池，最后尝试释放最久没用的池，最后执行<see cref="Trim"/>策略
            /// </summary>
            Hybrid,
        }
        
        // 对象名称到池子的缓存映射，用于快速查找链表
        private readonly Dictionary<string, LinkedListNode<IPool>> _pools = new();
        // 对象池的LRU链表，最近使用的在链表头
        private readonly LinkedList<IPool> _lruPoolIds = new();
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
            _isOpenLayout = GlobalSettings.Instance.poolModuleConfig.isOpenLayout;
            activeTimeThreshold = GlobalSettings.Instance.poolModuleConfig.activeTimeThreshold;
            poolMinSize = GlobalSettings.Instance.poolModuleConfig.poolMinSize;
            poolMaxSize = GlobalSettings.Instance.poolModuleConfig.poolMaxSize;
        }

        public T Get<T>(string key) where T : Object
        {
            // 存在该对象就取出来使用
            if (!_pools.TryGetValue(key, out var monoNode)) 
                return null;
            
            InsertFirst(monoNode);
            var cacheObj = ((IPool<T>)monoNode.Value).Get();
            return cacheObj;
        }

        public void PushObj<T>(T obj) where T : Object
        {
            if (!obj)
            {
                Logger.LogError(ELogTags.Pool, $"The object to be cached is null.");
                return;
            }
            
            // 第一次缓存对象时初始化缓存池存储结构
            if (!_poolRootObj && _isOpenLayout)
            {
                _poolRootObj = new GameObject("Pool");
                Object.DontDestroyOnLoad(_poolRootObj);
            }
            
            // 已经存储过了就可以直接往容器中存储对象
            if (_pools.TryGetValue(obj.name, out var monoNode))
            {
                InsertFirst(monoNode);
                var monoPool = monoNode.Value;
                ((IPool<T>)monoPool).Push(obj);
            }
            else
            {
                // 第一次存储要创建存储容器
                var newObjectPool = new ObjectPool<T>(_poolRootObj, obj.name, _isOpenLayout, activeTimeThreshold, poolMinSize, poolMaxSize);
                var newNode = new LinkedListNode<IPool>(newObjectPool);
                newObjectPool.Push(obj);
                // 先插入到链表头
                InsertFirst(newNode);
                // 缓存字典
                _pools.Add(obj.name, newNode);
            }
        }

        /// <summary>
        /// 移动到链表头
        /// </summary>
        /// <param name="poolNode"></param>
        private void InsertFirst(LinkedListNode<IPool> poolNode)
        {
            // 新节点先插入链表，所以链表中不存在这个节点，需要判断不是新节点才去移除
            if (_pools.ContainsKey(poolNode.Value.PoolId))
            {
                _lruPoolIds.Remove(poolNode);
            }
            _lruPoolIds.AddFirst(poolNode);
        }
        
        public T GetData<T>() where T : class, IPoolData
        {
            // 自定义获取名称，与存储名称一致
            var dataName = $"{typeof(T).FullName}";
            T data = null;
            if (_pools.TryGetValue(dataName, out var dataPoolNode) && dataPoolNode.Value is DataPool<T> dataPool)
            {
                InsertFirst(dataPoolNode);
                data = dataPool.Get();
            }

            data ??= DIContainer.Create<T>();
            
            // 注入内容
            DIContainer.InjectIntoInstance(data);
            return data;
        }

        public void PushData<T>(T data) where T : class, IPoolData
        {
            if(data == null)
                return;
            
            // 自定义缓存名称，与获取名称一致
            var dataName = $"{typeof(T).FullName}";
            if (_pools.TryGetValue(dataName, out var dataPoolNode))
            {
                (dataPoolNode.Value as DataPool<T>)?.Push(data);
                InsertFirst(dataPoolNode);
            }
            else
            {
                var newDataPool = new DataPool<T>(activeTimeThreshold, poolMinSize, poolMaxSize);
                newDataPool.Push(data);
                dataPoolNode = new LinkedListNode<IPool>(newDataPool);
                // 先插入到链表头
                InsertFirst(dataPoolNode);
                _pools.Add(dataName, dataPoolNode);
            }
        }

        /// <summary>
        /// 获取指定资源缓存的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetUnUsedCount(string key)
        {
            return _pools.TryGetValue(key, out var poolNode) ? poolNode.Value.InactiveCount : 0;
        }
        
        public int ReleaseCache(string key)
        {
            if (!_pools.TryGetValue(key, out var poolNode))
            {
                return 0;
            }

            var pool = poolNode.Value;
            var count = pool.InactiveCount;
            pool.ClearAll();
            _pools.Remove(key);
            return count;
        }
        
        public void ClearAll()
        {
            foreach (var node in _pools.Values)
            {
                node.Value.ClearAll();
            }
            
            _pools.Clear();
            EngineUtility.Destroy(_poolRootObj);
            _poolRootObj = null;
            GC.Collect();
        }

        /// <summary>
        /// 强制释放内存，可指定释放的选择策略
        /// </summary>
        /// <param name="disposalStrategy">释放策略</param>
        /// <param name="executeCount">执行次数，即释放的池子数量，仅当EReleaseStrategy为LRU时使用</param>
        public void ReleaseCache(EReleaseStrategy disposalStrategy, ushort executeCount = 5)
        {
            if(_pools.Count == 0)
                return;

            switch (disposalStrategy)
            {
                case EReleaseStrategy.Trim:
                    // 裁剪所有池子缓存
                    foreach (var poolNode in _pools.Values)
                    {
                        poolNode.Value.Trim();
                    }
                    break;
                case EReleaseStrategy.Hybrid:
                    var releaseCount = executeCount;
                    var cur = _lruPoolIds.Last;
                    while (cur != null && releaseCount > 0)
                    {
                        var pre = cur.Previous;
                        var releaseId = cur.Value.PoolId;
                        var releasePool = _pools[releaseId].Value;
                        // 先释放未使用的惰性池
                        if (releasePool.IsLazy && releasePool.ActiveCount == 0)
                        {
                            _lruPoolIds.RemoveLast();
                            releasePool.ClearAll();
                            _pools.Remove(releaseId);
                            --releaseCount;
                        }

                        cur = pre;
                    }

                    // 未达执行次数，降级处理，释放未使用的池
                    if (releaseCount > 0)
                    {
                        var dels = new List<LinkedListNode<IPool>>(_pools.Count);
                        foreach (var (id, poolNode) in _pools)
                        {
                            if (poolNode.Value.ActiveCount == 0)
                            {
                                dels.Add(poolNode);
                                --releaseCount;
                            }
                            
                            if(releaseCount == 0)
                                return;
                        }
                    
                        foreach (var node in dels)
                        {
                            _pools.Remove(node.Value.PoolId);
                            _lruPoolIds.Remove(node);
                        }
                    }

                    // 未达执行次数，降级处理，释放最久没用的池
                    if (releaseCount > 0)
                    {
                        cur = _lruPoolIds.Last;
                        while (cur != null && releaseCount > 0)
                        {
                            var pre = cur.Previous;
                            var releaseId = cur.Value.PoolId;
                            var releasePool = _pools[releaseId];
                            _lruPoolIds.RemoveLast();
                            releasePool.Value.ClearAll();
                            _pools.Remove(releaseId);
                            --releaseCount;
                            cur = pre;
                        }
                    }

                    ReleaseCache(EReleaseStrategy.Trim);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(disposalStrategy), disposalStrategy, null);
            }
        }

        public void OnReport(MemoryReportData memoryReportData)
        {
            switch (memoryReportData.level)
            {
                case EMemoryOccupationLevel.Normal:
                    return;
                case EMemoryOccupationLevel.Warning:
                    ReleaseCache(EReleaseStrategy.Trim);
                    break;
                case EMemoryOccupationLevel.Critical:
                    ReleaseCache(EReleaseStrategy.Hybrid);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
