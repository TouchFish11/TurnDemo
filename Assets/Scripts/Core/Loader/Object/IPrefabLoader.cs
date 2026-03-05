using System.Threading.Tasks;
using UnityEngine;

namespace Core.Loader.Object
{
    /// <summary>
    /// 预制体加载器接口
    /// </summary>
    public interface IPrefabLoader : IAssetLoader
    {
        /// <summary>
        /// 异步获取对象
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="parent"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="worldPosStay"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> GetObjectAsync<T>(string abName, string assetName, Transform parent, Vector3 pos, Quaternion rot, bool worldPosStay = false) where T : Component;
        
        /// <summary>
        /// 异步获取对象
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="parent"></param>
        /// <param name="worldPosStay"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> GetObjectAsync<T>(string abName, string assetName, Transform parent, bool worldPosStay = false) where T : Component;

        /// <summary>
        /// 异步获取游戏对象
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="parent"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="worldPosStay"></param>
        /// <returns></returns>
        Task<GameObject> GetGameObjectAsync(string abName, string assetName, Transform parent, Vector3 pos, Quaternion rot, bool worldPosStay = false);
        
        /// <summary>
        /// 异步获取游戏对象
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="parent"></param>
        /// <param name="worldPosStay"></param>
        /// <returns></returns>
        Task<GameObject> GetGameObjectAsync(string abName, string assetName, Transform parent, bool worldPosStay = false);
        
        /// <summary>
        /// 回收资源
        /// </summary>
        /// <param name="gameObject"></param>
        void CollectAsset(GameObject gameObject);
        
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        void RealseAsset(string abName, string assetName);
    }
}
