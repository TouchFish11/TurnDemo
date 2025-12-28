using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// 输入管理器接口
/// </summary>
public interface IInputManager
{
    void EditInput(Key oldKeyBoard, UnityAction callBack);
    void InitSystem();
    void StartOrCloseInput(bool isStart);
}
