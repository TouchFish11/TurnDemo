using System;
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
        /// <param name="projectileTrans"></param>
        /// <param name="data"></param>
        /// <param name="action"></param>
        /// <param name="vFXFlag"></param>
        /// <returns></returns>
        void CreateVFX(string vfxName, ProjectileTrans projectileTrans, ProjectileData data, VFXInfo vFXFlag);

        /// <summary>
        /// 移除指定名称的所有特效
        /// </summary>
        /// <param name="vFXInfo"></param>
        void RemoveVFX(VFXInfo vFXInfo);

        /// <summary>
        /// 清空特效缓存
        /// </summary>
        void ClearVFXCache();
    }
}
