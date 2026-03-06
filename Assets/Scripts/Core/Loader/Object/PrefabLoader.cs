using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Log;
using Core.Pool;
using Core.Service;
using Core.Tasks.Extensions;
using UnityEngine;

namespace Core.Loader.Object
{
    /// <summary>
    /// 预制体加载器
    /// </summary>
    public class PrefabLoader : IPrefabLoader
    {
        private class PrefabData : IPoolData
        {
            /// <summary>
            /// 预制体资源
            /// </summary>
            public GameObject objAsset;
            
            /// <summary>
            /// 该资源的引用计数，实例化数
            /// </summary>
            public int refCount;

            public PrefabData Init(GameObject objAsset, int refCount)
            {
                this.objAsset = objAsset;
                this.refCount = refCount;
                return this;
            }

            public void ResetData()
            {
                objAsset = null;
                refCount = 0;
            }
        }
        
        // AB包管理器接口
        private readonly IAssetBundleManager _assetBundleManager = ServiceLocator.Get<IAssetBundleManager>();
        // 缓存池接口
        private readonly IPoolManager _poolManager = ServiceLocator.Get<IPoolManager>();
        // 资源名称到预制体数据映射
        private readonly Dictionary<string, PrefabData> _assetNameToData = new();

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="parent"></param>
        /// <param name="worldPosStay"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> GetObjectAsync<T>(string abName, string assetName, Transform parent, bool worldPosStay = false) where T : class
        {
            return await GetObjectAsync<T>(abName, assetName, parent, Vector3.zero, Quaternion.identity, worldPosStay);
        }
        
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="parent"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="worldPosStay"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> GetObjectAsync<T>(string abName, string assetName, Transform parent, Vector3 pos, Quaternion rot, bool worldPosStay = false) where T : class
        {
            var instanceObj = await GetGameObjectAsyncInternal(abName, assetName);
            // 设置父对象、位置
            instanceObj.transform.SetParent(parent, worldPosStay);
            instanceObj.transform.SetLocalPositionAndRotation(pos, rot);
            return instanceObj.GetComponent<T>();
        }

        public Task<GameObject> GetGameObjectAsync(string abName, string assetName, Transform parent, bool worldPosStay = false)
        {
            return GetGameObjectAsync(abName, assetName, parent, Vector3.zero, Quaternion.identity, worldPosStay);
        }
        
        public async Task<GameObject> GetGameObjectAsync(string abName, string assetName, Transform parent, Vector3 pos, Quaternion rot, bool worldPosStay = false)
        {
            var instanceObj = await GetGameObjectAsyncInternal(abName, assetName);
            // 设置父对象、位置
            instanceObj.transform.SetParent(parent, worldPosStay);
            instanceObj.transform.SetLocalPositionAndRotation(pos, rot);
            return instanceObj;
        }

        /// <summary>
        /// 异步获取游戏对象
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        internal async Task<GameObject> GetGameObjectAsyncInternal(string abName, string assetName)
        {
            // 从缓存池获取
            var instanceObj = _poolManager.GetAssetBundleObj(abName, assetName);
            // 不存在可复用的该对象
            if (instanceObj)
            {
                return instanceObj;
            }
            
            // 是否已存在资源缓存
            if (_assetNameToData.TryGetValue(assetName, out var prefabData))
            {
                // 实例化预设体
                instanceObj = UnityEngine.Object.Instantiate(prefabData.objAsset);
                // 资源引用数+1
                prefabData.refCount += 1;
            }
            else
            {
                // AB包异步加载
                var assetBundle = await _assetBundleManager.LoadBundleAsync(abName);
                var objAsset = await assetBundle.LoadAssetAsync<GameObject>(assetName).ToTask<GameObject>();
                // 缓存加载的资源
                if (!_assetNameToData.TryAdd(assetName, _poolManager.GetData<PrefabData>().Init(objAsset, 1)))
                {
                    _assetNameToData[assetName].refCount += 1;
                }
                // 实例化预设体
                instanceObj = UnityEngine.Object.Instantiate(objAsset);
            }

            // 避免实例化出的对象的名字后带有(Clone)
            instanceObj.name = assetName;
            return instanceObj;
        }
        
        public void CollectAsset(GameObject gameObject)
        {
            _poolManager.PushObj(gameObject);
        }
        
        public void RealseAsset(string abName, string assetName)
        {
            if (!_assetNameToData.TryGetValue(assetName, out var prefabData))
            {
                return;
            }

            var unUsedCount = _poolManager.GetUnUsedCount(assetName);
            if (prefabData.refCount != unUsedCount)
            {
                LogManager.LogWarning($"{nameof(PrefabLoader)}.{nameof(RealseAsset)}：无法释放该{abName}.{assetName}资源。引用数：{prefabData.refCount}，未使用数：{unUsedCount}");
                return;
            }
            
            _poolManager.ClearCache(assetName);
            prefabData.objAsset = null;
            _assetNameToData.Remove(assetName);
            _assetBundleManager.UnloadBundle(abName);
        }
    }
}
