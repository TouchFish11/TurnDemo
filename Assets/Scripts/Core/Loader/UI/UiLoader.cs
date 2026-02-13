using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Pool;
using Core.Service;
using Core.UI;
using UnityEngine;

namespace Core.Loader.UI
{
    /// <summary>
    /// UI加载器
    /// </summary>
    public class UiLoader : IUiLoader
    {
        public async Task<T> GetUIObject<T>(EAssetBundleType assetBundleType, string assetName, Transform parent, bool worldPosStay = false) where T : class, IUiBehaviour
        {
            return await GetUIObject<T>(assetBundleType, assetName, parent, Vector3.zero, Quaternion.identity, worldPosStay);
        }
        
        public async Task<T> GetUIObject<T>(EAssetBundleType assetBundleType, string assetName, Transform parent, Vector3 pos, Quaternion rot, bool worldPosStay = false) where T : class, IUiBehaviour
        {
            var cacheObj = await ServiceLocator.Get<IPoolManager>().GetAssetBundleObjAsync(assetBundleType, assetName);
            cacheObj.transform.SetParent(parent, worldPosStay);
            cacheObj.transform.SetLocalPositionAndRotation(pos, rot);
            return cacheObj.TryGetComponent(out T component) ? component : null;
        }
        
        public async Task<GameObject> GetUIGameobject(EAssetBundleType assetBundleType, string assetName, Transform parent, bool worldPosStay = false)
        {
            var cacheObj = await ServiceLocator.Get<IPoolManager>().GetAssetBundleObjAsync(assetBundleType, assetName);
            cacheObj.transform.SetParent(parent, worldPosStay);
            return cacheObj;
        }
    }
}
