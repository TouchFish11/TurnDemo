using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 事件中心接口
/// </summary>
public interface IEventCenter
{
    void AddEventListener(E_EventType eventType, UnityAction callBack);
    void AddEventListener<T>(E_EventType eventType, UnityAction<T> callBack);
    void Clear();
    void DelayTriggerEvent(E_EventType eventType);
    void DelayTriggerEvent(E_EventType eventType, object info);
    void RemoveEventListener(E_EventType eventType, UnityAction callBack);
    void RemoveEventListener<T>(E_EventType eventType, UnityAction<T> callBack);
    void RemoveEventsFrom(E_EventType eventType);
    void TriggerEvent(E_EventType eventType);
    void TriggerEvent<T>(E_EventType eventType, T info);
}
