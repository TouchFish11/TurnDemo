using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 鼠标可见性变化事件
/// </summary>
public class MouseVisibleChangedEvent : IEvent
{
    public string SourceName { get; }

    public bool IsVisible { get; }

    public MouseVisibleChangedEvent(string sourceName, bool isVisible)
    {
        SourceName = sourceName;
        IsVisible = isVisible;
    }

    void IEvent.ResetEvent()
    {

    }
}
