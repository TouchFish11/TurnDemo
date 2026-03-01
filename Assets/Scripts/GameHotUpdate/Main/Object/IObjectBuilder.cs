using System.Threading.Tasks;
using UnityEngine;

namespace GameHotUpdate.Main.Object
{
    public interface IObjectBuilder
    {
        /// <summary>
        /// 获取热更对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="parent"></param>
        /// <param name="worldPosStay"></param>
        /// <returns></returns>
        Task<T> GetHotfixObject<T>(string abName, string assetName, Transform parent, bool worldPosStay = false) where T : MonoBehaviour;
        
        /// <summary>
        /// 获取指定GameObject
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="parent"></param>
        /// <param name="worldPosStay"></param>
        /// <returns></returns>
        Task<GameObject> GetGameobject(string abName, string assetName, Transform parent, bool worldPosStay = false);
    }
}
