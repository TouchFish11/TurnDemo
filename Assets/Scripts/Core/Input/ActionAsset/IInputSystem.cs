using System;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Core.Input.ActionAsset
{
    /// <summary>
    /// ����ϵͳ�ӿ�
    /// </summary>
    public interface IInputSystem
    {
        void DisableInput();
        void EditInput(E_MainActionMap keyMap, Key oldKey, UnityAction<E_KeyConflict> overCallBack);
        void EnableInput();
        InputAction GetInputAction(string actionName);
        Task InitPlayerInput(string abName, PlayerInput playerInput, MainActionMapDataContainer container,
            Action<InputAction.CallbackContext> onActionTrigger);
        void InvokeExchangeKey();
        void UpdateActions(PlayerInput playerInput = null);
    }
}
