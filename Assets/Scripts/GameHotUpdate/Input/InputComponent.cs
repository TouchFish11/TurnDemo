using System;
using System.Collections.Generic;
using Core.Components;
using Core.GlobalEvent;
using Core.GlobalEvent.Events;
using Core.Input.ActionAsset;
using Core.Log;
using Core.Mono;
using Core.Service;
using Game.Components;
using Game.Input;
using Game.Manager;
using GameHotUpdate.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace GameHotUpdate.Input
{
    /// <summary>
    /// 输入组件，负责处理玩家的各类输入事件（键鼠、摇杆等），并对外暴露输入相关的事件回调
    /// </summary>
    [ComponentId(typeof(InputComponent))]
    [RequireComponent(typeof(PlayerInputComponent))]
    public class InputComponent : BaseComponent
    {
        // 输入系统接口，封装底层输入逻辑
        private IInputSystem inputSystem;
        // 受限制的输入动作名称列表
        private readonly List<string> actionNmaes = new();
        // 输入限制计数（用于判断是否有输入限制生效）
        private byte inputLimitCount;

        /// <summary>
        /// 是否处于输入限制状态（有任意输入限制生效时为true）
        /// </summary>
        private bool IsLimitInput => inputLimitCount != 0;

        /// <summary>
        /// 键盘移动输入变更事件（参数为移动方向的三维向量，y轴固定为0）
        /// </summary>
        public event Action<Vector3> OnKeyInputChanged;
    
        /// <summary>
        /// 鼠标滑动（移动）变更事件（参数为鼠标滑动的二维向量）
        /// </summary>
        public event Action<Vector2> OnMouseSlideChanged;

        /// <summary>
        /// 鼠标左键点击（普攻）事件
        /// </summary>
        public event Action OnMouseLeftClick;

        /// <summary>
        /// 鼠标滚轮滚动事件（参数为滚轮滚动的数值）
        /// </summary>
        public event Action<float> OnScrollWheel;

        /// <summary>
        /// 交互操作事件（如与场景物体交互）
        /// </summary>
        public event Action OnIniteract;

        /// <summary>
        /// 组件初始化方法（继承自BaseComponent）
        /// </summary>
        /// <param name="entityObject">所属的实体对象</param>
        public override async void Init(IEntityObject entityObject)
        {
            try
            {
                // 获取输入系统实例
                inputSystem = ServiceLocator.Get<IInputSystem>();
                // 初始化玩家输入，并注册输入动作触发回调
                var container = ServiceLocator.Get<IGameManager>().GameDataManager.InputActionContainer;
                await inputSystem.InitPlayerInput(AbKeyCollection.Gameconfig, EntityObject.GetComponent<PlayerInputComponent>().PlayerInput, container, OnActionTrigger);
                // 添加帧更新监听，处理每帧的输入逻辑
                ServiceLocator.Get<IMonoAdapter>().AddUpdateListener(OnUpdate);
            }
            catch (Exception e)
            {
                // 记录初始化异常日志
                LogManager.LogError($"{nameof(InputComponent)}.{nameof(Init)} error: {e.Message}");
            }
        }

        /// <summary>
        /// 添加输入限制（指定输入动作将被限制，仅允许受限列表内的输入生效）
        /// </summary>
        /// <param name="actionName">需要限制的输入动作名称</param>
        public void LimitInput(string actionName)
        {
            inputLimitCount++;
            actionNmaes.Add(actionName);
        }

        /// <summary>
        /// 取消指定输入动作的限制
        /// </summary>
        /// <param name="actionName">需要取消限制的输入动作名称</param>
        public void CancelLimitInput(string actionName)
        {
            // 从限制列表移除动作名称，成功则减少限制计数
            if (actionNmaes.Remove(actionName))
            {
                inputLimitCount--;
            }
        }

        /// <summary>
        /// 启用输入系统（恢复所有输入响应）
        /// </summary>
        public void EnableInput()
        {
            inputSystem.EnableInput();
        }

        /// <summary>
        /// 禁用输入系统（停止所有输入响应）
        /// </summary>
        public void DisEnableInput()
        {
            inputSystem.DisableInput();
        }

        /// <summary>
        /// 输入动作触发回调方法（由输入系统调用）
        /// </summary>
        /// <param name="context">输入动作的上下文信息</param>
        private void OnActionTrigger(InputAction.CallbackContext context)
        {
            // 若处于输入限制状态，且当前触发的动作不在受限列表中，则忽略该输入
            if (IsLimitInput && !ContainInputName(context.action.name))
            {
                return;
            }

            // 根据输入动作名称分发处理逻辑
            switch (context.action.name)
            {
                case "Move":
                    // 移动输入：执行阶段触发时读取输入值，非执行阶段重置为零向量
                    if (context.phase == InputActionPhase.Performed)
                    {
                        var input = context.ReadValue<Vector2>();
                        OnKeyInputChanged?.Invoke(new Vector3(input.x, 0, input.y));
                    }
                    else
                    {
                        OnKeyInputChanged?.Invoke(Vector3.zero);
                    }
                    break;
                case "NormalAttack" when !ServiceLocator.Get<IMouseManager>().Visible:
                    // 普通攻击（鼠标左键）：鼠标不可见时触发
                    if (context.phase == InputActionPhase.Performed)
                    {
                        LogManager.Log($"触发普攻");
                        OnMouseLeftClick?.Invoke();
                    }
                    break;
                case "Interact":
                    // 交互操作：执行阶段触发
                    if(context.phase == InputActionPhase.Performed)
                    {
                        OnIniteract?.Invoke();
                    }
                    break;
                case "MouseMove":
                    // 鼠标移动：实时触发，传递鼠标滑动向量
                    OnMouseSlideChanged?.Invoke(context.ReadValue<Vector2>());
                    break;
                case "ScrollZoom":
                    // 滚轮缩放：传递滚轮滚动数值
                    OnScrollWheel?.Invoke(context.ReadValue<float>());
                    break;
                case "MouseVisible":
                    // 鼠标显隐切换（左Alt键触发）
                    switch (context.phase)
                    {
                        case InputActionPhase.Started:
                            // 开始按压：触发鼠标显示事件
                            ServiceLocator.Get<IEventCenter>().TriggerEvent(new MouseVisibleChangedEvent { SourceName = nameof(Keyboard.current.leftAltKey), IsVisible = true});
                            break;
                        case InputActionPhase.Canceled:
                            // 取消按压：触发鼠标隐藏事件
                            ServiceLocator.Get<IEventCenter>().TriggerEvent(new MouseVisibleChangedEvent { SourceName = nameof(Keyboard.current.leftAltKey), IsVisible = false });
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// 检查指定输入动作名称是否在受限列表中
        /// </summary>
        /// <param name="actionName">输入动作名称</param>
        /// <returns>存在返回true，否则返回false</returns>
        public bool ContainInputName(string actionName)
        {
            return actionNmaes.Contains(actionName);
        }

        /// <summary>
        /// 帧更新方法（每帧调用）
        /// </summary>
        private void OnUpdate()
        {
            // 理论流程：先释放鼠标，再显示对应UI；实际流程：先显示UI，关闭UI后，再释放UI，然后再释放鼠标（由于关闭UI走下面的逻辑释放的鼠标）
            
            // 检测鼠标左键抬起
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                // 点击UI时的逻辑，若按下Alt后点击鼠标，点击到了UI界面，应该触发Alt鼠标隐藏事件，因为鼠标点击后MouseVisible的取消回调不会触发
                // 只在抬起时检测是否在UI上，来判断是否释放因为Alt键请求的显示
                if (EventSystem.current && EventSystem.current.IsPointerOverGameObject())
                {
                    ServiceLocator.Get<IEventCenter>().TriggerEvent(new MouseVisibleChangedEvent
                    {
                        SourceName = nameof(Keyboard.current.leftAltKey), 
                        IsVisible = false 
                    });
                }
            }
        }

        /// <summary>
        /// 组件销毁时的清理方法
        /// </summary>
        private void OnDestroy()
        {
            // 清空所有事件回调，避免内存泄漏
            OnKeyInputChanged = null;
            OnMouseSlideChanged = null;
            OnMouseLeftClick = null;
            OnMouseLeftClick = null;
            OnIniteract = null;
        }
    }
}