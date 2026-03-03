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
        /// 用于希望将obj对象放入缓存池的情况
        /// </summary>
        /// <param name="abName">资源AB包名称</param>
        /// <param name="obj">被缓存的对象</param>
        void RealseAsset(string abName, GameObject obj);

        /// <summary>
        /// 释放资源
        /// 用于不希望将obj放入缓存池的情况
        /// </summary>
        /// <param name="abName">资源AB包名称</param>
        /// <param name="assetName">资源名称</param>
        void RealseAsset(string abName, string assetName);
    }
}
