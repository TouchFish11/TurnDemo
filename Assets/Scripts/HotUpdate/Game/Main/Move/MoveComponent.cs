using HotUpdate.Base.ECModule;
using HotUpdate.Game.Cameras;
using UnityEngine;

namespace HotUpdate.Game.Main.Move
{
    /// <summary>
    /// 移动组件
    /// 负责控制游戏实体的移动、旋转逻辑，响应输入指令并结合相机视角计算移动方向
    /// </summary>
    [ComponentId]
    [ComponentCore(typeof(MoveComponentCore))]
    [RequireComponent(typeof(CharacterControllerComponent))]
    public class MoveComponent : BaseComponent
    {
        private MoveComponentCore _moveComponentCore;
        
        protected override void OnInit()
        {
            _moveComponentCore = (MoveComponentCore)ComponentCore;
        }
        
        public void SetCamera(OrbitCameraController camera)
        {
            _moveComponentCore.SetCamera(camera);
        }

        /// <summary>
        /// 启用移动功能
        /// </summary>
        public void Enable()
        {
            _moveComponentCore.Enable();
        }

        /// <summary>
        /// 禁用移动功能
        /// 同时重置移动方向，防止禁用后仍有残留移动逻辑
        /// </summary>
        public void Disable()
        {
            _moveComponentCore.Disable();
        }

        /// <summary>
        /// 外部设置移动开关状态
        /// </summary>
        /// <param name="canMove">是否允许移动</param>
        public void SetMoveFlag(bool canMove)
        {
            _moveComponentCore.SetMoveFlag(canMove);
        }

        protected override void OnBaseDestroy()
        {
            _moveComponentCore = null;
        }
    }
}