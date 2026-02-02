using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Core.Input.ActionAsset
{
    /// <summary>
    /// 键位路径映射结构
    /// </summary>
    [Serializable]
    public struct KeyPathMap
    {
        /// <summary>
        /// 键盘键位
        /// </summary>
        public Key key;

        /// <summary>
        /// 鼠标按钮
        /// </summary>
        public MouseButton mouseButton;

        /// <summary>
        /// 鼠标值
        /// </summary>
        public E_MouseValue mouseValue;

        /// <summary>
        /// 路径
        /// </summary>
        public string path;

        public KeyPathMap(Key key, string path)
        {
            this.key = key;
            this.path = path;
            mouseButton = MouseButton.Left;
            mouseValue = E_MouseValue.None;
        }

        public KeyPathMap(MouseButton mouseButton, string path)
        {
            this.mouseButton = mouseButton;
            this.path = path;
            key = Key.None;
            mouseValue = E_MouseValue.None;
        }

        public KeyPathMap(E_MouseValue mouseValue, string path)
        {
            this.mouseValue = mouseValue;
            this.path = path;
            key = Key.None;
            mouseButton = MouseButton.Left;
        }
    }
}
