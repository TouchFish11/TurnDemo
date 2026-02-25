using System.Threading.Tasks;
using UnityEngine;

namespace Core.Loader.UI
{
    public interface IUiLoader : IAssetLoader
    {
        /// <summary>
        /// 获取指定的UI
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="parent"></param>
        /// <param name="worldPosStay"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> GetUIObject<T>(string abName, string assetName, Transform parent, bool worldPosStay = false) where T : class;
        
        /// <summary>
        /// 获取指定的UI
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="parent"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="worldPosStay"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> GetUIObject<T>(string abName, string assetName, Transform parent, Vector3 pos, Quaternion rot, bool worldPosStay = false) where T : class;
        
        /// <summary>
        /// 获取指定的UIGameObject
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="parent"></param>
        /// <param name="worldPosStay"></param>
        /// <returns></returns>
        Task<GameObject> GetUIGameobject(string abName, string assetName, Transform parent, bool worldPosStay = false);

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        void RealseAsset(string abName, string assetName);
    }
}
