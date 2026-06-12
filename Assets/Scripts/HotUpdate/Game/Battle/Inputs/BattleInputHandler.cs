using System;
using Core.DI;
using Core.Mono;
using Core.Mono.MonoFunction;
using Core.Serialize.Binary;
using HotUpdate.Base;
using HotUpdate.Base.Manager;
using HotUpdate.Common.Config.ExcelInfo.Container;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Skill;
using UnityEngine;
using UnityEngine.InputSystem;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Battle.Inputs
{
    /// <summary>
    /// 战斗场景输入处理器
    /// 负责处理战斗中的鼠标拖拽、点击选择目标、技能释放目标选择等核心输入逻辑
    /// 继承自单例自动挂载 MonoBehaviour 基类，保证全局唯一且自动初始化
    /// </summary>
    public class BattleInputHandler : IBattleInputHandler, IDisposable
    {
        [Inject] private IMonoAdapter _monoAdapter;
        [Inject] private IBinaryDataManager _binaryDataManager;

        private BattleCoordinator _battleCoordinator;
        // 拖拽起始位置（屏幕坐标）
        private Vector3 _dragStartPosition;
        // 是否处于拖拽状态（用于区分点击和拖拽行为）
        private bool _isDragging;
        // 拖拽阈值（超过该距离判定为拖拽，否则为点击）
        private const float dragThreshold = 50f;
        // 激活拖拽的最小偏移
        private const float activateThreshold = 4f;

        // 上一帧鼠标X
        private float lastMouseX;          
        // 累计偏移量
        private float nowDeltaX;
        // 能否输入
        private bool _canInput;
        
        private Action _OnLeftDrag;
        private Action _OnRightDrag;
        
        /// <summary>
        /// 向左拖拽的事件（用于切换目标等逻辑）
        /// </summary>
        public event Action OnLeftDrag
        {
            add
            {
                if (_OnLeftDrag != null)
                {
                    Logger.LogError($"{nameof(OnLeftDrag)}重复添加");
                    return;
                }
                _OnLeftDrag += value;
            }
            remove => _OnLeftDrag -= value;
        }

        /// <summary>
        /// 向右拖拽的事件（用于切换目标等逻辑）
        /// </summary>
        public event Action OnRightDrag
        {
            add
            {
                if (_OnRightDrag != null)
                {
                    Logger.LogError($"{nameof(OnRightDrag)}重复添加");
                    return;
                }
                _OnRightDrag += value;
            }
            remove => _OnRightDrag -= value;
        }

        /// <summary>
        /// 拖拽过程中的事件（传递拖拽X轴偏移量）
        /// 事件参数：拖拽X轴方向的偏移量（像素）
        /// </summary>
        public event Action<float> OnDrag;
        
        /// <summary>
        /// 是否触发回弹
        /// </summary>
        public event Action<bool> OnRebound;

        /// <summary>
        /// 点击事件
        /// </summary>
        public event Action OnClick;

        public BattleInputHandler(BattleCoordinator battleCoordinator)
        {
            _battleCoordinator = battleCoordinator;
            // 注册帧更新监听，每帧执行输入处理逻辑
            _monoAdapter.AddUpdateListener(OnUpdate);
        }

        /// <summary>
        /// 设置输入状态
        /// </summary>
        /// <param name="activeInput"></param>
        public void SetInputState(bool activeInput)
        {
            _canInput = activeInput;
        }
        
        /// <summary>
        /// 帧更新回调方法
        /// 统一调度输入处理逻辑
        /// </summary>
        private void OnUpdate()
        {
            if (!_canInput)
            {
                return;
            }
            
            InputHandle();
        }
        
        /// <summary>
        /// 核心输入处理逻辑
        /// 处理鼠标左键的按下、拖拽、抬起全生命周期逻辑
        /// 区分拖拽（切换目标/技能）和点击（选择技能释放目标）行为
        /// </summary>
        private void InputHandle()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // 记录按下时的鼠标屏幕坐标，作为拖拽起始点
                _dragStartPosition = Input.mousePosition;
            }
            
            if (Mouse.current.leftButton.isPressed)
            {
                // 未进入拖拽状态时，判断鼠标移动距离是否超过阈值，超过则标记为拖拽状态
                if (!_isDragging && Vector2.Distance(Input.mousePosition, _dragStartPosition) > activateThreshold)
                {
                    nowDeltaX = 0;
                    lastMouseX = Input.mousePosition.x;
                    _isDragging = true;
                }

                // 处于拖拽状态时，处理拖拽偏移逻辑
                if (_isDragging)
                {
                    // 获取当前鼠标X，计算【本次帧内】的偏移增量（核心修正）
                    var currentMouseX = Input.mousePosition.x;
                    var deltaX = currentMouseX - lastMouseX;
                    lastMouseX = currentMouseX; // 更新上一帧鼠标X

                    // 累加帧内增量到总偏移
                    nowDeltaX += deltaX;

                    // 触发拖拽中事件，传递X轴偏移量
                    OnDrag?.Invoke(deltaX);

                    // 偏移量超过阈值时，触发左右拖拽事件并重置起始位置（避免重复触发）
                    if (Mathf.Abs(nowDeltaX) > dragThreshold)
                    {
                        // 向右拖拽：触发右拖拽事件
                        if (nowDeltaX > 0)
                        {
                            _OnRightDrag?.Invoke();
                        }
                        // 向左拖拽：触发左拖拽事件
                        else if (nowDeltaX < 0)
                        {
                            _OnLeftDrag?.Invoke();
                        }

                        nowDeltaX = 0;
                    }
                }
            }
            
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                // 若是拖曳中释放鼠标，则回弹
                if (_isDragging)
                {
                    OnRebound?.Invoke(true);
                    // 重置拖拽状态，结束本次拖拽
                    _isDragging = false;
                    return;
                }
                
                // 触发点击事件
                OnClick?.Invoke();
            }
        }

        public void Dispose()
        {
            // 移除帧更新监听
            _monoAdapter.RemoveUpdateListener(OnUpdate);
            // 清空事件委托，避免空引用和内存泄漏
            _OnLeftDrag = null;
            _OnRightDrag = null;
        }
    }
}