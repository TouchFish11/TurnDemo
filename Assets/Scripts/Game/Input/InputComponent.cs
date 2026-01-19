using Framework;
using Game;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 输入组件
/// </summary>
[RequireComponent(typeof(PlayerInput))]
[ComponentId(nameof(InputComponent))]
public class InputComponent : BaseComponent
{
    // 输入系统接口
    private IInputSystem inputSystem;
    // 输入动作名称列表
    private List<string> actionNmaes = new List<string>();
    // 允许的输入数量
    private byte inputLimitCount;

    /// <summary>
    /// 是否限制输入
    /// </summary>
    private bool IsLimitInput => inputLimitCount != 0;

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
        inputSystem = ServiceLocator.Get<IInputSystem>();
        await inputSystem.InitPlayerInput(this.EntityObject.GetComponent<PlayerInput>(), OnActionTrigger);
        ServiceLocator.Get<IMonoManager>().AddUpdateListener(OnUpdate);
    }

    /// <summary>
    /// 限制输入
    /// </summary>
    /// <param name="actionName"></param>
    public void LimitInput(string actionName)
    {
        inputLimitCount++;
        actionNmaes.Add(actionName);
    }

    /// <summary>
    /// 取消限制输入
    /// </summary>
    /// <param name="actionName"></param>
    public void CancelLimitInput(string actionName)
    {
        if (actionNmaes.Remove(actionName))
        {
            inputLimitCount--;
        }
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
        // 过滤输入
        if (IsLimitInput && !ContainInputName(context.action.name))
        {
            return;
        }

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
            case "NormalAttack" when !ServiceLocator.Get<IMouseManager>().Visible:

                if (context.phase == InputActionPhase.Performed)
                {
                    LogManager.Log($"普攻触发");
                    OnMouseLeftClick?.Invoke();
                }
                break;
            case "Interact":
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
                    EventCenter.Instance.TriggerEvent(new MouseVisibleChangedEvent() { SourceName = nameof(Keyboard.current.leftAltKey), IsVisible = true});
                }
                else if(context.phase == InputActionPhase.Canceled)
                {
                    // FIXME：鼠标可见后，触发鼠标点击，会导致该状态无法进入，无法准确隐藏鼠标
                    EventCenter.Instance.TriggerEvent(new MouseVisibleChangedEvent() { SourceName = nameof(Keyboard.current.leftAltKey), IsVisible = false });
                }
                break;
        }
    }

    /// <summary>
    /// 是否包含输入的名称
    /// </summary>
    /// <param name="actionName"></param>
    /// <returns></returns>
    public bool ContainInputName(string actionName)
    {
        return actionNmaes.Contains(actionName);
    }

    /// <summary>
    /// 帧更新
    /// </summary>
    private void OnUpdate()
    {
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
