using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.UI;
using UnityEngine;

namespace Core.Loader.UI
{
    public interface IUiLoader : IAssetLoader
    {
        /// <summary>
        /// 获取指定的UI
        /// </summary>
        /// <param name="assetBundleType"></param>
        /// <param name="assetName"></param>
        /// <param name="parent"></param>
        /// <param name="worldPosStay"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> GetUIObject<T>(EAssetBundleType assetBundleType, string assetName, Transform parent, bool worldPosStay = false) where T : class, IUiBehaviour;
        
        /// <summary>
        /// 获取指定的UI
        /// </summary>
        /// <param name="assetBundleType"></param>
        /// <param name="assetName"></param>
        /// <param name="parent"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="worldPosStay"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> GetUIObject<T>(EAssetBundleType assetBundleType, string assetName, Transform parent, Vector3 pos, Quaternion rot, bool worldPosStay = false) where T : class, IUiBehaviour;
        
        /// <summary>
        /// 获取指定的UIGameObject
        /// </summary>
        /// <param name="assetBundleType"></param>
        /// <param name="assetName"></param>
        /// <param name="parent"></param>
        /// <param name="worldPosStay"></param>
        /// <returns></returns>
        Task<GameObject> GetUIGameobject(EAssetBundleType assetBundleType, string assetName, Transform parent, bool worldPosStay = false);

    }
}
