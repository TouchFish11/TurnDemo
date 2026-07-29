using System;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Core.Inputs
{
    /// <summary>
    /// 输入系统接口
    /// </summary>
    public interface IInputSystem
    {
        void DisableInput();
        void EditInput(E_MainActionMap keyMap, Key oldKey, UnityAction<E_KeyConflict> overCallBack);
        void EnableInput();
        InputAction GetInputAction(string actionName);
        void InvokeExchangeKey();
        void UpdateActions(PlayerInput playerInput = null);
        
        /// <summary>
        /// 初始化玩家输入组件
        /// </summary>
        /// <param name="playerInput">玩家输入组件实例</param>
        /// <param name="container">主行动映射数据容器</param>
        /// <param name="onActionTrigger">输入动作触发时的回调方法</param>
        /// <returns>异步任务</returns>
        void InitPlayerInput(PlayerInput playerInput, MainActionMapDataContainer container, Action<InputAction.CallbackContext> onActionTrigger);

        /// <summary>
        /// 初始化输入系统
        /// </summary>
        /// <param name="inputJson">输入配置JSON</param>
        void InitInputSystem(string inputJson);
    }
}