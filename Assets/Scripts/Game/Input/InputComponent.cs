using Framework;
using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// 输入组件
/// </summary>
public class InputComponent : BaseComponent
{
    // 当前输入
    private Vector3 currentInput;
    // 能否输入
    private bool enableInput;

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

    protected override void Awake()
    {
        base.Awake();

        MonoManager.Instance.AddUpdateListener(OnUpdate);
        enableInput = true;
    }

    public void EnableInput()
    {
        enableInput = true;
    }

    public void DisEnableInput()
    {
        enableInput = false;
    }

    /// <summary>
    /// 帧更新
    /// </summary>
    private void OnUpdate()
    {
        if (!enableInput)
        {
            return;
        }

        // 键盘输入
        float h = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 newInput = new Vector3(h, 0, z).normalized;
        if (newInput != currentInput)
        {
            OnKeyInputChanged?.Invoke(newInput);
            currentInput = newInput;
        }

        // 鼠标滑动输入
        float x = Input.GetAxisRaw("Mouse X");
        float y = Input.GetAxisRaw("Mouse Y");

        Vector2 newMouseInput = new Vector2(x, y);
        OnMouseSlideChanged?.Invoke(newMouseInput);

        if (Keyboard.current.leftAltKey.isPressed)
        {
            MouseManager.Instance.RequestMouseVisible(nameof(Keyboard.current.leftAltKey));
        }
        else
        {
            MouseManager.Instance.ReleaseMouseVisible(nameof(Keyboard.current.leftAltKey));
        }

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
        OnKeyInputChanged = null;
        OnMouseSlideChanged = null;
        OnMouseLeftClick = null;
    }
}
