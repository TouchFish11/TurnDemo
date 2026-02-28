using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Log;
using Core.Pool;
using Core.Service;
using UnityEngine;

namespace Core.Loader.UI
{
    /// <summary>
    /// UI加载器
    /// </summary>
    public class UiLoader : IUiLoader
    {
        // 资源名称到引用计数映射
        private readonly Dictionary<string, int> _nameToUiRef = new();
        // AB包管理器接口
        private readonly IAssetBundleManager _assetBundleManager;
        
        public UiLoader()
        {
            _assetBundleManager = ServiceLocator.Get<IAssetBundleManager>();
        }
        
        public async Task<T> GetUIObject<T>(string abName, string assetName, Transform parent, bool worldPosStay = false) where T : class
        {
            return await GetUIObject<T>(abName, assetName, parent, Vector3.zero, Quaternion.identity, worldPosStay);
        }
        
        public async Task<T> GetUIObject<T>(string abName, string assetName, Transform parent, Vector3 pos, Quaternion rot, bool worldPosStay = false) where T : class
        {
            var cacheObj = await ServiceLocator.Get<IPoolManager>().GetAssetBundleObjAsync(abName, assetName);
            cacheObj.transform.SetParent(parent, worldPosStay);
            cacheObj.transform.SetLocalPositionAndRotation(pos, rot);
            // 更新引用计数
            AddRefCount(assetName);
            return cacheObj.TryGetComponent(out T component) ? component : null;
        }
        
        public async Task<GameObject> GetUIGameobject(string abName, string assetName, Transform parent, bool worldPosStay = false)
        {
            var cacheObj = await ServiceLocator.Get<IPoolManager>().GetAssetBundleObjAsync(abName, assetName);
            cacheObj.transform.SetParent(parent, worldPosStay);
            // 更新引用计数
            AddRefCount(assetName);
            return cacheObj;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        public void RealseAsset(string abName, string assetName)
        {
            if (!_nameToUiRef.ContainsKey(assetName))
            {
                return;
            }
            
            if (--_nameToUiRef[assetName] != 0)
            {
                return;
            }
            
            _nameToUiRef.Remove(assetName);
            _assetBundleManager.UnloadBundle(abName);
        }

        /// <summary>
        /// 添加资源引用计数
        /// </summary>
        /// <param name="assetName"></param>
        private void AddRefCount(string assetName)
        {
            if (!_nameToUiRef.TryAdd(assetName, 1))
            {
                ++_nameToUiRef[assetName];
            }
        }
    }
}
