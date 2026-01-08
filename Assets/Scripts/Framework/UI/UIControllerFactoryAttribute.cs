using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI控制器工厂特效
/// 用于标记UI控制器，和其对应的工厂
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class UIControllerFactoryAttribute : Attribute
{
    public Type ControllerFactory { get; }

    public UIControllerFactoryAttribute(Type controllerFactory)
    {
        this.ControllerFactory = controllerFactory;
    }
}
