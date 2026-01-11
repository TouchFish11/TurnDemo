using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态类型ID特性
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class StatusTypeIdAttribute : Attribute
{
    public int StatusId { get; }

    public StatusTypeIdAttribute(int statusId)
    {
        this.StatusId = statusId;
    }
}
