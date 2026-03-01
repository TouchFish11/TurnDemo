using UnityEngine;

namespace GameHotUpdate.VFX
{
    /// <summary>
    /// ��Ч�������ӿ�
    /// </summary>
    public interface IVFXManager
    {
        /// <summary>
        /// ��ȡ��Ч
        /// </summary>
        /// <param name="vfxName"></param>
        /// <param name="projectileTrans"></param>
        /// <param name="data"></param>
        /// <param name="vFXFlag"></param>
        /// <returns></returns>
        void CreateVFX(string vfxName, ProjectileTrans projectileTrans, ProjectileData data, VFXInfo vFXFlag);

        /// <summary>
        /// �Ƴ�ָ�����Ƶ�������Ч
        /// </summary>
        /// <param name="vFXInfo"></param>
        void RemoveVFX(VFXInfo vFXInfo);

        /// <summary>
        /// �����Ч����
        /// </summary>
        void ClearVFXCache();

        void CreateVFX(string vfxName, Transform parent, Vector3 pos, Quaternion rot, VFXInfo vFXInfo);
    }
}
