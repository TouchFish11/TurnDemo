using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// 输入系统接口
/// </summary>
public interface IInputSystem
{
    void DisableInput();
    void EditInput(E_MainActionMap keyMap, Key oldKey, UnityAction<E_KeyConflict> overCallBack);
    void EnableInput();
    InputAction GetInputAction(string actionName);
    Task InitPlayerInput(PlayerInput playerInput, Action<InputAction.CallbackContext> onActionTrigger);
    void InvokeExchangeKey();
    void UpdateActions(PlayerInput playerInput = null);
}
