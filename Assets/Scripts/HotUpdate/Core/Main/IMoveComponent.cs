using Core.Components;

namespace HotUpdate.Core.Main
{
    public interface IMoveComponent : IComponent
    {
        /// <summary>
        /// 启用移动功能
        /// </summary>
        void Enable();

        /// <summary>
        /// 禁用移动功能
        /// 同时重置移动方向，防止禁用后仍有残留移动逻辑
        /// </summary>
        void Disable();

        /// <summary>
        /// 外部设置移动开关状态
        /// </summary>
        /// <param name="canMove">是否允许移动</param>
        void SetMoveFlag(bool canMove);
    }
}
