using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace Framework.InputSystem
{
    /// <summary>
    /// 输入动作数据容器
    /// </summary>
    [Serializable]
    public sealed class InputActionContainer
    {
        //存储输入动作数据
        private Dictionary<E_KeyMap, (Key key, string path)> _inputActinoDic = new Dictionary<E_KeyMap, (Key, string)>();

        public InputActionContainer()
        {
            InputActionData data = new InputActionData();

            //示例
            // _inputActinoDic.Add(E_KeyMap.Jump, (Key.Space, data.jump));
            //...
        }

        /// <summary>
        /// 输入动作数据容器类对象
        /// </summary>
        public Dictionary<E_KeyMap, (Key key, string path)> InputActinoDic { get => _inputActinoDic; }
    }

    /// <summary>
    /// 输入动作数据
    /// </summary>
    [Serializable]
    public sealed class InputActionData
    {
        //private string _jump = "<Keyboard>/Space";

        //public string jump {  get => _jump; }
    }
}
