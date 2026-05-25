using System;
using Core.Components;
using UnityEngine;

namespace HotUpdate.Game.Inputs
{
    public interface IInputComponent : IComponent
    {
        /// <summary>
        /// 键盘移动输入变更事件（参数为移动方向的三维向量，y轴固定为0）
        /// </summary>
        event Action<Vector3> OnKeyInputChanged;

        /// <summary>
        /// 鼠标滑动（移动）变更事件（参数为鼠标滑动的二维向量）
        /// </summary>
        event Action<Vector2> OnMouseSlideChanged;

        /// <summary>
        /// 鼠标左键点击（普攻）事件
        /// </summary>
        event Action OnMouseLeftClick;

        /// <summary>
        /// 鼠标滚轮滚动事件（参数为滚轮滚动的数值）
        /// </summary>
        event Action<float> OnScrollWheel;

        /// <summary>
        /// 交互操作事件（如与场景物体交互）
        /// </summary>
        event Action OnIniteract;

        /// <summary>
        /// 添加输入限制（指定输入动作将被限制，仅允许受限列表内的输入生效）
        /// </summary>
        /// <param name="actionName">需要限制的输入动作名称</param>
        void LimitInput(string actionName);

        /// <summary>
        /// 取消指定输入动作的限制
        /// </summary>
        /// <param name="actionName">需要取消限制的输入动作名称</param>
        void CancelLimitInput(string actionName);

        /// <summary>
        /// 启用输入系统（恢复所有输入响应）
        /// </summary>
        void EnableInput();

        /// <summary>
        /// 禁用输入系统（停止所有输入响应）
        /// </summary>
        void DisEnableInput();

        /// <summary>
        /// 检查指定输入动作名称是否在受限列表中
        /// </summary>
        /// <param name="actionName">输入动作名称</param>
        /// <returns>存在返回true，否则返回false</returns>
        bool ContainInputName(string actionName);
    }
}
