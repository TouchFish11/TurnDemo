using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DI;
using Core.Mono;
using Core.Pool;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Logger = Core.Log.Logger;
using Object = UnityEngine.Object;

namespace Core.AssetBundles.Management
{
    /// <summary>
    /// 对象生成器
    /// </summary>
    public class ObjectSpawner : IDisposable, IPoolData
    {
        [Inject] private IPoolManager _poolManager;
        // 资源Key到资源句柄的映射
        private readonly Dictionary<string, AssetHandle> _keyToHandleMap = new();
        // 资源Key到资源加载任务的映射
        private readonly Dictionary<string, Task<AssetHandle<GameObject>>> _keyToHandleTaskMap = new();
        // 缓存加载过的资源Key
        private readonly HashSet<string> _assetKeys = new();

        /// <summary>
        /// 生成对象
        /// </summary>
        /// <param name="key">资源Key</param>
        /// <param name="parent">对象的父对象</param>
        /// <param name="pos">若是UI对象，则为锚点坐标；否则根据父对象是否存在来设置本地/世界坐标</param>
        /// <param name="rot">若是UI对象，则为本地旋转；否则根据父对象是否存在来设置本地/世界旋转</param>
        /// <param name="worldSpace">是否保留世界坐标</param>
        /// <typeparam name="T">游戏对象上的组件类型</typeparam>
        /// <returns>返回该游戏对象上的特定组件</returns>
        public PoolObject<T> Spawn<T>(string key, Transform parent = null, Vector3 pos = default, Quaternion rot = default, bool worldSpace = false) where T : Object
        {
            // 尝试从对象池获取
            var poolObject = GetPoolObject<T>(key, parent, pos, rot, worldSpace);
            if(poolObject.Obj)
                return poolObject.Convert<T>();

            // 复用句柄资源实例化
            if (_keyToHandleMap.TryGetValue(key, out var handle))
            {
                return Instantiate<T>(handle, key, parent, pos, rot, worldSpace);
            }
            
            // 加载资源
            handle = GameAsset.LoadAsset<GameObject>(key);
            // 缓存Key
            _assetKeys.Add(key);
            // 缓存句柄
            _keyToHandleMap.Add(key, handle);
            Logger.Log($"[{nameof(ObjectSpawner)}]: Cache asset({key}), Cache handle id({handle.HandleId})");
            // 实例化
            return Instantiate<T>(handle, key, parent, pos, rot, worldSpace);
        }
        
        /// <summary>
        /// 异步生成对象
        /// </summary>
        /// <param name="key">资源Key</param>
        /// <param name="parent">对象的父对象</param>
        /// <param name="pos">若是UI对象，则为锚点坐标；否则根据父对象是否存在来设置本地/世界坐标</param>
        /// <param name="rot">若是UI对象，则为本地旋转；否则根据父对象是否存在来设置本地/世界旋转</param>
        /// <param name="worldSpace">是否保留世界坐标</param>
        /// <typeparam name="T">游戏对象上的组件类型</typeparam>
        /// <returns>返回该游戏对象上的特定组件</returns>
        public async Task<PoolObject<T>> SpawnAsync<T>(string key, Transform parent = null, Vector3 pos = default, Quaternion rot = default, bool worldSpace = false) where T : Object
        {
            // 尝试从对象池获取
            var poolObject = GetPoolObject<T>(key, parent, pos, rot, worldSpace);
            if(poolObject.Obj)
                return poolObject.Convert<T>();

            // 先从缓存句柄中获取
            if (_keyToHandleMap.TryGetValue(key, out var assetHandle))
            {
                // 实例化资源
                return Instantiate<T>(assetHandle, key, parent, pos, rot, worldSpace);
            }
            
            // 返回正在加载的任务
            if (_keyToHandleTaskMap.TryGetValue(key, out var cacheTask))
            {
                var handle = await cacheTask;
                return Instantiate<T>(handle, key, parent, pos, rot, worldSpace);
            }

            // 异步加载资源
            var handleTask = GameAsset.LoadAssetAsync<GameObject>(key);
            if (!_keyToHandleTaskMap.TryAdd(key, handleTask))
            {
                handleTask = _keyToHandleTaskMap[key];
            }

            try
            {
                // 等待资源加载
                var newHandle = await handleTask;
                // 缓存Key
                _assetKeys.Add(key);
                // 缓存句柄
                _keyToHandleMap.Add(key, newHandle);
                // 实例化资源
                return Instantiate<T>(newHandle, key, parent, pos, rot, worldSpace);
            }
            catch (Exception e)
            {
                GameAsset.Release(_keyToHandleMap[key]);
                _assetKeys.Remove(key);
                _keyToHandleMap.Remove(key);
                Logger.LogError($"[{nameof(ObjectSpawner)}]: {e.Message}");
                return default;
            }
            finally
            {
                _keyToHandleTaskMap.Remove(key);
            }
        }
        
        /// 从对象池复用对象为池化对象
        private PoolObject GetPoolObject<T>(string key, Transform parent = null, 
            Vector3 pos = default, Quaternion rot = default, bool worldSpace = false) where T : Object
        {
            var instance = _poolManager.Get<T>(key);
            if (!instance) 
                return default;
            
            var poolObject = new PoolObject(ObjectIdPool.GetGlobalId(), instance, this);
            switch (instance)
            {
                // UI
                case UIBehaviour uiBehaviour:
                {
                    var rectTransform = uiBehaviour.GetComponent<RectTransform>();
                    // 默认设置锚点坐标和本地旋转
                    rectTransform.SetParent(parent, worldSpace);
                    rectTransform.anchoredPosition = pos;
                    rectTransform.localRotation = rot;
                    break;
                }
                // 非UI：组件、GameObject
                default:
                {
                    var transform = instance.GetComponent<Transform>();
                    // 没有父对象，设置为世界坐标
                    if (!parent)
                    {
                        transform.position = pos;
                        transform.rotation = rot;
                    }
                    // 有父对象，设置为本地坐标
                    else
                    {
                        transform.SetParent(parent, worldSpace);
                        transform.localPosition = pos;
                        transform.localRotation = rot;
                    }
                    break;
                }
            }
            
            return poolObject.Convert<T>();
        }

        /// 实例化资源并封装为池化对象返回
        private PoolObject<T> Instantiate<T>(AssetHandle assetHandle, string key, Transform parent = null, 
            Vector3 pos = default, Quaternion rot = default, bool worldSpace = false) where T : Object
        {
            // 实例化资源
            var newObj = InstantiateInternal<T>(assetHandle, key, parent, pos, rot, worldSpace);
            var poolObject = new PoolObject(ObjectIdPool.GetGlobalId(), newObj, this);
            return poolObject.Convert<T>();
        }

        /// 实例化资源（内部）
        private static T InstantiateInternal<T>(AssetHandle assetHandle, string key, Transform parent = null, 
            Vector3 pos = default, Quaternion rot = default, bool worldSpace = false) where T : Object
        {
            // 实例化对象
            var newObj = Object.Instantiate(assetHandle.ConvertTo<T>().Asset);
            // 不是UI对象
            if (newObj is not UIBehaviour uiBehaviour)
            {
                // 没有父对象，设置为世界坐标
                if (!parent)
                {
                    var transform = newObj.GetComponent<Transform>();
                    transform.position = pos;
                    transform.rotation = rot;
                }
                // 有父对象，设置为本地坐标
                else
                {
                    var transform = newObj.GetComponent<Transform>();
                    transform.SetParent(parent, worldSpace);
                    transform.localPosition = pos;
                    transform.localRotation = rot;
                }
            }
            // UI对象
            else
            {
                // 默认设置锚点坐标和本地旋转
                uiBehaviour.transform.SetParent(parent, worldSpace);
                uiBehaviour.GetComponent<RectTransform>().anchoredPosition = pos;
                uiBehaviour.GetComponent<RectTransform>().localRotation = rot;
            }
            
            // 修改对象名称为资源唯一Key
            newObj.name = key;
            return newObj;
        }
        
        /// <summary>
        /// 异步生成多个对象，只能获取同一类型的多个资源，不支持混合类型
        /// </summary>
        /// <param name="keys">同一类型的不同资源key</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public async Task<PoolObject<T>> SpawnsAsync<T>(params string[] keys) where T : Object
        {
            var poolObject = new PoolObject(ObjectIdPool.GetGlobalId(), null, this);
            // 保存所有生成任务
            var loadTasks = new List<Task<PoolObject<T>>>();
            foreach (var key in keys)
            {
                loadTasks.Add(SpawnAsync<T>(key));
            }
            
            // 等待所有加载任务结束
            var poolObjects = await Task.WhenAll(loadTasks);
            
            // 存储结果到新池化对象中
            foreach (var po in poolObjects)
            {
                poolObject.Objs.Add(po.Obj);
            }

            return poolObject.Convert<T>();
        }

        /// <summary>
        /// 统一的回收入口（通过 PooledObject 自动调用）
        /// </summary>
        /// <param name="poolObject">池化对象</param>
        /// <param name="destroy">是否销毁不放入对象池</param>
        internal void Release(PoolObject poolObject, bool destroy)
        {
            if (poolObject.Objs.Count > 0)
            {
                foreach (var poolObjectObj in poolObject.Objs)
                {
                    ReleaseInternal(poolObjectObj, destroy);
                }
            }
            else
            {
                ReleaseInternal(poolObject.Obj, destroy);
            }
        }

        /// 释放对象（内部）
        private void ReleaseInternal(Object obj, bool destroy)
        {
            if (!obj)
            {
                Logger.LogError($"{nameof(ObjectSpawner)}: Manually destroying object is not allowed");
                return;
            }
            
            if (destroy)
                EngineUtility.Destroy(obj);
            else
            {
                Logger.Log($"[{nameof(ObjectSpawner)}]: {obj.name} collect pool");
                _poolManager.PushObj(obj);
            }
        }
        
        /// <summary>
        /// 销毁生成器，当不在使用该生成器时调用此方法，释放缓存的剩余句柄
        /// 要先确保生成器创建出的池化对象都执行回收后才能调用此方法销毁
        /// 否则对象池会有残留
        /// </summary>
        public void Dispose()
        {
            _poolManager.PushData(this);
        }

        void IPoolData.ResetData()
        {
            // 为了避免引用泄露，需要在不使用该生成器时统一释放剩余的句柄
            foreach (var handle in _keyToHandleMap.Values)
            {
                GameAsset.Release(handle);
            }
            _keyToHandleMap.Clear();
            
            // 清空对象池的这些资源Key的缓存对象
            foreach (var assetKey in _assetKeys)
            {
                _poolManager.ClearCache(assetKey);
            }
            _assetKeys.Clear();
        }
    }
}
