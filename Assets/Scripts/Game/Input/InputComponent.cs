using Framework;
using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

/// <summary>
/// 输入组件
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class InputComponent : BaseComponent
{
     /// <summary>
    /// 键盘输入改变事件
    /// </summary>
    public event UnityAction<Vector3> OnKeyInputChanged;
    
    /// <summary>
    /// 鼠标滑动改变事件
    /// </summary>
    public event UnityAction<Vector2> OnMouseSlideChanged;

    /// <summary>
    /// 鼠标左键点击事件
    /// </summary>
    public event UnityAction OnMouseLeftClick;

    /// <summary>
    /// 鼠标滚轮事件
    /// </summary>
    public event Action<float> OnScrollWheel;

    protected override async void Awake()
    {
        base.Awake();
        await Framework.InputSystem.Instance.InitPlayerInput(this.EntityObject.GetComponent<PlayerInput>(), OnActionTrigger);
        MonoManager.Instance.AddUpdateListener(OnUpdate);
    }

    // 限制输入
    public void LimitInput()
    {

    }

    // 取消限制输入
    public void CancelLimitInput()
    {

    }

    public async void EnableInput()
    {
        Framework.InputSystem.Instance.EnableInput();
    }

    public void DisEnableInput()
    {
        Framework.InputSystem.Instance.DisableInput();
    }

    private void OnActionTrigger(InputAction.CallbackContext context)
    {
        switch (context.action.name)
        {
            case "Move":
                if (context.phase == InputActionPhase.Performed)
                {
                    Vector2 input = context.ReadValue<Vector2>();
                    OnKeyInputChanged?.Invoke(new Vector3(input.x, 0, input.y));
                }
                else
                {
                    OnKeyInputChanged?.Invoke(Vector3.zero);
                }
                break;
            case "NormalAttack" when !ServiceLocator.Instance.Get<IMouseManager>().Visible:
                if (context.phase == InputActionPhase.Performed)
                {
                    this.EntityObject.GetComponent<AnimComponent>().OnAttack();
                }
                break;
            case "Initeract":
                this.EntityObject.GetComponent<InteractComponent>().Initeract();
                break;
            case "MouseMove":
                OnMouseSlideChanged?.Invoke(context.ReadValue<Vector2>());
                break;
            case "ScrollZoom":
                OnScrollWheel?.Invoke(context.ReadValue<float>());
                break;
            case "MouseVisible":
                if (context.phase == InputActionPhase.Performed)
                {
                    ServiceLocator.Instance.Get<IMouseManager>().RequestMouseVisible(nameof(Keyboard.current.leftAltKey));
                }
                else
                {
                    ServiceLocator.Instance.Get<IMouseManager>().ReleaseMouseVisible(nameof(Keyboard.current.leftAltKey));
                }
                break;
        }
    }

    /// <summary>
    /// 帧更新
    /// </summary>
    private void OnUpdate()
    {
        //// 暂时这样写，对话需要F键输入。之后在修改。
        //// F键交互
        //if (Keyboard.current.fKey.wasPressedThisFrame)
        //{
        //    this.EntityObject.GetComponent<InteractComponent>().Initeract();
        //}

        //// 键盘输入
        //float h = Input.GetAxisRaw("Horizontal");
        //float z = Input.GetAxisRaw("Vertical");

        //Vector3 newInput = new Vector3(h, 0, z).normalized;
        //if (newInput != currentInput)
        //{
        //    OnKeyInputChanged?.Invoke(newInput);
        //    currentInput = newInput;
        //}

        //// 鼠标滑动输入
        //float x = Input.GetAxisRaw("Mouse X");
        //float y = Input.GetAxisRaw("Mouse Y");

        //Vector2 newMouseInput = new Vector2(x, y);
        //OnMouseSlideChanged?.Invoke(newMouseInput);

        //if (Keyboard.current.leftAltKey.isPressed)
        //{
        //    MouseManager.Instance.RequestMouseVisible(nameof(Keyboard.current.leftAltKey));
        //}
        //else
        //{
        //    MouseManager.Instance.ReleaseMouseVisible(nameof(Keyboard.current.leftAltKey));
        //}

        // 鼠标左键点击输入
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // 落在UI上
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {

            }
            else
            {
                OnMouseLeftClick?.Invoke();
            }
        }
    }

    private void OnDestroy()
    {
        Framework.InputSystem.Instance.OnActionTrigger -= OnActionTrigger;

        OnKeyInputChanged = null;
        OnMouseSlideChanged = null;
        OnMouseLeftClick = null;
    }
}
