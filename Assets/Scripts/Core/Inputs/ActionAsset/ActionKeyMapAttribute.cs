using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Core.Inputs.ActionAsset
{
    /// <summary>
    /// 标记动作路径映射替换关键字
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ActionKeyMapAttribute : Attribute
    {
        public Key Key { get; }

        public MouseButton MouseButton { get; }

        public E_MouseValue MouseValue { get; }

        public ActionKeyMapAttribute(Key key)
        {
            Key = key;
        }

        public ActionKeyMapAttribute(MouseButton mouseButton)
        {
            MouseButton = mouseButton;
        }

        public ActionKeyMapAttribute(E_MouseValue mouseValue)
        {
            MouseValue = mouseValue;
        }
    }
}
