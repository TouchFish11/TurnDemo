using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Mono管理器接口
/// </summary>
public interface IMonoManager
{
    void AddFixedUpdateListener(UnityAction fixedUpdateFun);
    void AddLateUpdateListener(UnityAction lateUpdateFun);
    void AddUpdateListener(UnityAction updateFun);
    void RemoveFixedUpdateListener(UnityAction fixedUpdateFun);
    void RemoveLateUpdateListener(UnityAction lateUpdateFun);
    void RemoveUpdateListener(UnityAction updateFun);
    Coroutine StartCoroutine(IEnumerator coroutine);
}
