using Core.Components;
using UnityEngine;

namespace HotUpdate.Base.Camera
{
    /// <summary>
    /// 相机控制器接口
    /// </summary>
    public interface IOrbitCameraController
    {
        Transform Transform { get; }

        /// <summary>
        /// 设置跟随目标
        /// </summary>
        /// <param name="entityObject"></param>
        void SetTarget(IEntityObject entityObject);
    }
}
