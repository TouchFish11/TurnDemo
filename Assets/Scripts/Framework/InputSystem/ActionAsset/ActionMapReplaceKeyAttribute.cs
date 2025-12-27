using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 标记动作路径映射替换关键字
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ActionMapReplaceKeyAttribute : Attribute
{
    public string ReplaceKey { get; }

    public ActionMapReplaceKeyAttribute(string replaceKey)
    {
        ReplaceKey = replaceKey;
    }
}
