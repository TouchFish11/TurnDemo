using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Framework
{
    /// <summary>
    /// 输入数据
    /// </summary>
    [Serializable]
    public sealed class InputData
    {
        //键盘枚举
        private Key _key;
        //鼠标枚举
        private MouseButton _mouse;
        //输入类型
        private E_InputType _inputType;
        //输入模式
        private E_InputMode _inputMode;

        /// <summary>
        /// 初始化键盘输入
        /// </summary>
        /// <param name="keyBoard">键盘枚举</param>
        /// <param name="inputMode">输入模式</param>
        public InputData(Key keyBoard, E_InputMode inputMode)
        {
            _key = keyBoard;
            _inputMode = inputMode;
            _inputType = E_InputType.Key;
        }

        /// <summary>
        /// 初始化鼠标输入
        /// </summary>
        /// <param name="mouseButton">鼠标枚举</param>
        /// <param name="inputMode">输入模式</param>
        public InputData(MouseButton mouseButton, E_InputMode inputMode)
        {
            _mouse = mouseButton;
            _inputMode = inputMode;
            _inputType = E_InputType.Mouse;
        }

        /// <summary>
        /// 键盘枚举
        /// </summary>
        public Key Key { get { return _key; } set { _key = value; } }
        /// <summary>
        /// 鼠标枚举
        /// </summary>
        public MouseButton Mouse { get { return _mouse; } }
        /// <summary>
        /// 输入类型
        /// </summary>
        public E_InputType InputType { get { return _inputType; } }
        /// <summary>
        /// 输入模式
        /// </summary>
        public E_InputMode InputMode { get { return _inputMode; } }
    }
}
