using System.Threading.Tasks;
using Core.AssetBundles.Management;
using UnityEngine;

namespace HotUpdate.Base.Manager
{
    /// <summary>
    /// 战斗相机管理器接口
    /// </summary>
    public interface IBattleCameraManager
    {
        /// <summary>
        /// 创建相机
        /// </summary>
        /// <param name="cameraTrans">相机父对象，传null则没有父对象，localPos/localRot就表示世界位置和旋转</param>
        /// <param name="localPos">本地/世界坐标</param>
        /// <param name="localRot">本地/世界旋转</param>
        /// <param name="mask">相机渲染的mask</param>
        /// <returns></returns>
        Task<UnityEngine.Camera> CreateCamera(Transform cameraTrans, Vector3 localPos, Quaternion localRot, int mask);
        
        /// <summary>
        /// 创建相机
        /// </summary>
        /// <param name="cameraTrans">相机父对象，传null则没有父对象，localPos/localRot就表示世界位置和旋转</param>
        /// <param name="localPos">本地/世界坐标</param>
        /// <param name="localRot">本地/世界旋转</param>
        /// <returns></returns>
        Task<UnityEngine.Camera> CreateCamera(Transform cameraTrans, Vector3 localPos, Quaternion localRot);

        /// <summary>
        /// 当前激活的相机
        /// </summary>
        PoolObject<UnityEngine.Camera> CurrentActiveCameraPoolObject { get; }
    }
}
