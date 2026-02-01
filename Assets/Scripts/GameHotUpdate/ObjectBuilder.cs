using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Pool;
using Core.Service;
using Core.UI;
using Game.Objects;
using UnityEngine;

namespace GameHotUpdate
{
    /// <summary>
    /// 对象构建器
    /// </summary>
    public class ObjectBuilder : IObjectBuilder
    {
        /// <summary>
        /// 获取热更对象
        /// </summary>
        /// <typeparam name="T">热更类型</typeparam>
        /// <param name="assetBundleType"></param>
        /// <param name="assetName"></param>
        /// <param name="parent"></param>
        /// <param name="worldPosStay"></param>
        /// <returns></returns>
        public async Task<T> GetHotfixObject<T>(EAssetBundleType assetBundleType, string assetName, Transform parent, bool worldPosStay = false) where T : MonoBehaviour
        {
            var cacheObj = await ServiceLocator.Get<IPoolManager>().GetAssetBundleObjAsync(assetBundleType, assetName);
            cacheObj.transform.SetParent(parent, worldPosStay);

            if (cacheObj.TryGetComponent(out T component))
            {
                return component;
            }

            var hotfixObject = cacheObj.AddComponent<T>();
            return hotfixObject;
        }
        
        public async Task<T> GetHotfixUIObject<T>(EAssetBundleType assetBundleType, string assetName, Transform parent, bool worldPosStay = false) where T : BaseUIBehaviour
        {
            var cacheObj = await ServiceLocator.Get<IPoolManager>().GetAssetBundleObjAsync(assetBundleType, assetName);
            cacheObj.transform.SetParent(parent, worldPosStay);

            if (cacheObj.TryGetComponent(out T component))
            {
                return component;
            }
            
            var uiObj = cacheObj.AddComponent<T>();
            return uiObj;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetBundleType"></param>
        /// <param name="assetName"></param>
        /// <param name="position"></param>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public static async Task<T> GetObject<T>(EAssetBundleType assetBundleType, string assetName, Vector3 position, Quaternion quaternion) where T : Component
        {
            var cacheObj = await ServiceLocator.Get<IPoolManager>().GetAssetBundleObjAsync(assetBundleType, assetName);
            cacheObj.transform.SetPositionAndRotation(position, quaternion);
            var component = cacheObj.AddComponent<T>();
            return component;
        }

        /// <summary>
        /// 获取指定GameObject
        /// </summary>
        /// <param name="assetBundleType"></param>
        /// <param name="assetName"></param>
        /// <param name="parent"></param>
        /// <param name="worldPosStay"></param>
        /// <returns></returns>
        public async Task<GameObject> GetGameobject(EAssetBundleType assetBundleType, string assetName, Transform parent, bool worldPosStay = false)
        {
            var cacheObj = await ServiceLocator.Get<IPoolManager>().GetAssetBundleObjAsync(assetBundleType, assetName);
            cacheObj.transform.SetParent(parent, worldPosStay);
            return cacheObj;
        }

        /// <summary>
        /// 获取指定GameObject
        /// </summary>
        /// <param name="assetBundleType"></param>
        /// <param name="assetName"></param>
        /// <param name="position"></param>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public static async Task<GameObject> GetGameobject(EAssetBundleType assetBundleType, string assetName, Vector3 position, Quaternion quaternion)
        {
            var cacheObj = await ServiceLocator.Get<IPoolManager>().GetAssetBundleObjAsync(assetBundleType, assetName);
            cacheObj.transform.SetPositionAndRotation(position, quaternion);
            return cacheObj;
        }
    }
}
