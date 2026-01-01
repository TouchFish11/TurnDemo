using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 组件名称ID特性
/// 被该特性修饰的类，会被ComponentFactory找到其Type并缓存
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ComponentIdAttribute : Attribute
{
    /// <summary>
    /// 组件唯一ID
    /// </summary>
    public string ComponentName { get; private set; }

    public ComponentIdAttribute(string componentName)
    {
        ComponentName = componentName;
    }
}
