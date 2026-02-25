using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Log;
using Core.Pool;
using Core.Service;
using UnityEngine;

namespace Core.Loader.Object
{
    /// <summary>
    /// 预制体加载器
    /// </summary>
    public class PrefabLoader : IPrefabLoader
    {
        // 资源名称到引用计数映射
        private readonly Dictionary<string, int> _nameToRef = new();
        // AB包管理器接口
        private readonly IAssetBundleManager _assetBundleManager;
        
        public PrefabLoader()
        {
            _assetBundleManager = ServiceLocator.Get<IAssetBundleManager>();
        }
        
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="parent"></param>
        /// <param name="worldPosStay"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> GetObject<T>(string abName, string assetName, Transform parent, bool worldPosStay = false) where T : class
        {
            return await GetObject<T>(abName, assetName, parent, Vector3.zero, Quaternion.identity, worldPosStay);
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
        public async Task<T> GetObject<T>(string abName, string assetName, Transform parent, Vector3 pos, Quaternion rot, bool worldPosStay = false) where T : class
        {
            var cacheObj = await ServiceLocator.Get<IPoolManager>().GetAssetBundleObjAsync(abName, assetName);
            cacheObj.transform.SetParent(parent, worldPosStay);
            cacheObj.transform.SetLocalPositionAndRotation(pos, rot);
            // 更新引用计数
            AddRefCount(assetName);
            return cacheObj.TryGetComponent(out T component) ? component : null;
        }
        
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        public void RealseAsset(string abName, string assetName)
        {
            if (!_nameToRef.ContainsKey(assetName))
            {
                return;
            }
            
            --_nameToRef[assetName];
            if (_nameToRef[assetName] != 0)
            {
                return;
            }
            
            _nameToRef.Remove(assetName);
            _assetBundleManager.UnloadBundle(abName);
        }
        
        /// <summary>
        /// 添加资源引用计数
        /// </summary>
        /// <param name="assetName"></param>
        private void AddRefCount(string assetName)
        {
            if (!_nameToRef.TryAdd(assetName, 1))
            {
                ++_nameToRef[assetName];
            }
        }
    }
}
