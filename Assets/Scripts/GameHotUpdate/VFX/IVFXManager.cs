using UnityEngine;

namespace GameHotUpdate.VFX
{
    /// <summary>
    /// 特效管理器接口
    /// </summary>
    public interface IVFXManager
    {
        /// <summary>
        /// 创建特效
        /// </summary>
        /// <param name="vfxName"></param>
        /// <param name="projectileTrans"></param>
        /// <param name="data"></param>
        /// <param name="vFXFlag"></param>
        void CreateVFX(string vfxName, ProjectileTrans projectileTrans, ProjectileData data, VFXInfo vFXFlag);

        /// <summary>
        /// 创建特效
        /// </summary>
        /// <param name="vfxName"></param>
        /// <param name="parent"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="vFXInfo"></param>
        void CreateVFX(string vfxName, Transform parent, Vector3 pos, Quaternion rot, VFXInfo vFXInfo);
        
        /// <summary>
        /// 移除特效
        /// </summary>
        /// <param name="vFXInfo"></param>
        void RemoveVFX(VFXInfo vFXInfo);

        /// <summary>
        /// 清理特效
        /// </summary>
        void ClearVFXCache();
    }
}
