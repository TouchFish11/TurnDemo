using System.Threading.Tasks;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 特效管理器接口
    /// </summary>
    public interface IVFXManager
    {
        /// <summary>
        /// 获取特效
        /// </summary>
        /// <param name="vfxName"></param>
        /// <param name="parent"></param>
        /// <param name="worldPositionStays"></param>
        /// <returns></returns>
        void CreateVFX(string vfxName, Transform parent, ProjectileData data, bool worldPositionStays = false);

        /// <summary>
        /// 获取特效
        /// </summary>
        /// <param name="vfxName"></param>
        /// <param name="worldPos"></param>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        void CreateVFX(string vfxName, Vector3 worldPos, Quaternion quaternion, ProjectileData data);

        /// <summary>
        /// 获取特效
        /// </summary>
        /// <param name="vfxName"></param>
        /// <param name="parent"></param>
        /// <param name="localPos"></param>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        void CreateVFX(string vfxName, Transform parent, Vector3 localPos, Quaternion quaternion, ProjectileData data, bool worldPositionStays = false);

        /// <summary>
        /// 移除指定激活的特效
        /// 移除不存在的特效不会报错
        /// </summary>
        /// <param name="vfxObj"></param>
        void RemoveActiveVFX(GameObject vfxObj);

        /// <summary>
        /// 移除指定名称的所有特效
        /// </summary>
        /// <param name="vfxName"></param>
        void RemoveVFX(string vfxName);

        /// <summary>
        /// 清空特效缓存
        /// </summary>
        void ClearVFXCache();
    }
}
