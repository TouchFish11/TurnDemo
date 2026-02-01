using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.UI;
using UnityEngine;

namespace Game.Objects
{
    public interface IObjectBuilder
    {
        /// <summary>
        /// 获取热更对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetBundleType"></param>
        /// <param name="assetName"></param>
        /// <param name="parent"></param>
        /// <param name="worldPosStay"></param>
        /// <returns></returns>
        Task<T> GetHotfixObject<T>(EAssetBundleType assetBundleType, string assetName, Transform parent, bool worldPosStay = false) where T : MonoBehaviour;
        
        /// <summary>
        /// 获取热更UI
        /// </summary>
        /// <param name="assetBundleType"></param>
        /// <param name="assetName"></param>
        /// <param name="parent"></param>
        /// <param name="worldPosStay"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> GetHotfixUIObject<T>(EAssetBundleType assetBundleType, string assetName, Transform parent, bool worldPosStay = false) where T : BaseUIBehaviour;
        
        /// <summary>
        /// 获取指定GameObject
        /// </summary>
        /// <param name="assetBundleType"></param>
        /// <param name="assetName"></param>
        /// <param name="parent"></param>
        /// <param name="worldPosStay"></param>
        /// <returns></returns>
        Task<GameObject> GetGameobject(EAssetBundleType assetBundleType, string assetName, Transform parent, bool worldPosStay = false);
        
    }
}
