using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Framework
{
    /// <summary>
    /// 输入动作数据容器
    /// </summary>
    [Serializable]
    public class InputActionContainer
    {
        /// <summary>
        /// 键位路基映射结构
        /// </summary>
        [Serializable]
        public struct KeyPathMap
        {
            public Key key;
            public MouseButton mouseButton;
            public string path;

            public KeyPathMap(Key key, string path)
            {
                this.key = key;
                this.path = path;
                this.mouseButton = MouseButton.Back;
            }

            public KeyPathMap(MouseButton mouseButton, string path)
            {
                this.mouseButton = mouseButton;
                this.path = path;
                this.key = Key.None;
            }
        }

        // 存储输入动作数据
        private readonly Dictionary<E_KeyMap, KeyPathMap> _inputActinoMap = new Dictionary<E_KeyMap, KeyPathMap>();

        public InputActionContainer()
        {
            InputActionData data = new InputActionData();

            _inputActinoMap.Add(E_KeyMap.Up, new KeyPathMap(Key.W, data.Up));
            _inputActinoMap.Add(E_KeyMap.Down, new KeyPathMap(Key.S, data.Down));
            _inputActinoMap.Add(E_KeyMap.Left, new KeyPathMap(Key.A, data.Left));
            _inputActinoMap.Add(E_KeyMap.Right, new KeyPathMap(Key.D, data.Right));
            _inputActinoMap.Add(E_KeyMap.Attack, new KeyPathMap(MouseButton.Left, data.Attack));
        }

        /// <summary>
        /// 输入动作数据字典
        /// </summary>
        public Dictionary<E_KeyMap, KeyPathMap> InputActinoDic => _inputActinoMap;
    }
}
