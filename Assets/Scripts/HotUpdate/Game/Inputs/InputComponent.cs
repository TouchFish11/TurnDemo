using System;
using HotUpdate.Base.ECModule;
using UnityEngine;

namespace HotUpdate.Game.Inputs
{
    /// <summary>
    /// 输入组件，负责处理玩家的各类输入事件（键鼠、摇杆等），并对外暴露输入相关的事件回调
    /// </summary>
    [ComponentId]
    [ComponentCore(typeof(InputComponentCore))]
    [RequireComponent(typeof(PlayerInputComponent))]
    public class InputComponent : BaseComponent
    {
        // 输入组件逻辑对象
        private InputComponentCore _inputComponentCore;

        protected override void OnInit()
        {
            _inputComponentCore = (InputComponentCore)ComponentCore;
        }

        /// <summary>
        /// 添加输入限制（指定输入动作将被限制，仅允许受限列表内的输入生效）
        /// </summary>
        /// <param name="actionName">需要限制的输入动作名称</param>
        public void LimitInput(string actionName)
        {
            _inputComponentCore.LimitInput(actionName);
        }

        /// <summary>
        /// 取消指定输入动作的限制
        /// </summary>
        /// <param name="actionName">需要取消限制的输入动作名称</param>
        public void CancelLimitInput(string actionName)
        {
            _inputComponentCore.CancelLimitInput(actionName);
        }

        /// <summary>
        /// 启用输入系统（恢复所有输入响应）
        /// </summary>
        public void EnableInput()
        {
            _inputComponentCore.EnableInput();
        }

        /// <summary>
        /// 禁用输入系统（停止所有输入响应）
        /// </summary>
        public void DisableInput()
        {
            _inputComponentCore.DisableInput();
        }
        
        /// <summary>
        /// 检查指定输入动作名称是否在受限列表中
        /// </summary>
        /// <param name="actionName">输入动作名称</param>
        /// <returns>存在返回true，否则返回false</returns>
        public bool ContainInputName(string actionName)
        {
            return _inputComponentCore.ContainInputName(actionName);
        }

        public void AddKeyInputChangedListener(Action<Vector3> action)
        {
            _inputComponentCore.OnKeyInputChanged += action;
        }
        
        public void AddMouseLeftClickListener(Action action)
        {
            _inputComponentCore.OnMouseLeftClick += action;
        }
        
        public void AddMouseSlideChangedListener(Action<Vector2> action)
        {
            _inputComponentCore.OnMouseSlideChanged += action;
        }
        
        public void AddIniteractListener(Action action)
        {
            _inputComponentCore.OnIniteract += action;
        }
        
        protected override void OnBaseDestroy()
        {
            _inputComponentCore = null;
        }
    }
}