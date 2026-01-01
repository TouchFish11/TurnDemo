using Framework;
using Game;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// 输入组件
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class InputComponent : BaseComponent
{
    private IInputSystem inputSystem;

    private bool isVisableMouse;

    /// <summary>
    /// 键盘输入改变事件
    /// </summary>
    public event Action<Vector3> OnKeyInputChanged;
    
    /// <summary>
    /// 鼠标滑动改变事件
    /// </summary>
    public event Action<Vector2> OnMouseSlideChanged;

    /// <summary>
    /// 鼠标左键点击事件
    /// </summary>
    public event Action OnMouseLeftClick;

    /// <summary>
    /// 鼠标滚轮事件
    /// </summary>
    public event Action<float> OnScrollWheel;

    /// <summary>
    /// 交互事件
    /// </summary>
    public event Action OnIniteract;

    public override async void Init(IEntityObject entityObject)
    {
        inputSystem = ServiceLocator.Instance.Get<IInputSystem>();
        await inputSystem.InitPlayerInput(this.EntityObject.GetComponent<PlayerInput>(), OnActionTrigger);
        ServiceLocator.Instance.Get<IMonoManager>().AddUpdateListener(OnUpdate);
    }

    // 限制输入
    public void LimitInput()
    {

    }

    // 取消限制输入
    public void CancelLimitInput()
    {

    }

    /// <summary>
    /// 启用输入系统
    /// </summary>
    public void EnableInput()
    {
        inputSystem.EnableInput();
    }

    /// <summary>
    /// 禁用输入系统
    /// </summary>
    public void DisEnableInput()
    {
        inputSystem.DisableInput();
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
                if (context.phase == InputActionPhase.Started)
                {
                    OnMouseLeftClick?.Invoke();
                }
                break;
            case "Initeract":
                if(context.phase == InputActionPhase.Performed)
                {
                    OnIniteract?.Invoke();
                }
                break;
            case "MouseMove":
                OnMouseSlideChanged?.Invoke(context.ReadValue<Vector2>());
                break;
            case "ScrollZoom":
                OnScrollWheel?.Invoke(context.ReadValue<float>());
                break;
            case "MouseVisible":
                if (context.phase == InputActionPhase.Started)
                {
                    EventCenter.Instance.TriggerEvent(E_EventType.E_MouseVisible, nameof(Keyboard.current.leftAltKey));
                    isVisableMouse = true;
                }
                else if(context.phase == InputActionPhase.Canceled)
                {
                    // FIXME：鼠标可见后，触发鼠标点击，会导致该状态无法进入，无法准确隐藏鼠标
                    EventCenter.Instance.TriggerEvent(E_EventType.E_MouseInvisible, nameof(Keyboard.current.leftAltKey));
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
        //    this.EntityObject.GetComponent<InteractComponent>().OnIniteract();
        //}

        // 鼠标左键点击输入
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            //// 落在UI上
            //if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            //{
            //    isVisableMouse = false;
            //    EventCenter.Instance.TriggerEvent(E_EventType.E_MouseInvisible, nameof(Keyboard.current.leftAltKey));
            //}
        }
    }

    private void OnDestroy()
    {
        OnKeyInputChanged = null;
        OnMouseSlideChanged = null;
        OnMouseLeftClick = null;
        OnMouseLeftClick = null;
        OnIniteract = null;
    }
}
