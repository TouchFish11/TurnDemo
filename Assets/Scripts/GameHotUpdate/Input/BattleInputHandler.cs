using System;
using Core.DataPersistence.Binary;
using Core.Log;
using Core.Mono;
using Core.Service;
using Core.Singleton;
using Game.Battle;
using Game.Battle.Context;
using Game.Battle.Input;
using Game.Battle.Objects;
using Game.Battle.Skill.Enum;
using GameHotUpdate.Battle.Event.UI;
using GameHotUpdate.Layer;
using GameHotUpdate.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameHotUpdate.Input
{
    /// <summary>
    /// 战斗场景输入处理器
    /// 负责处理战斗中的鼠标拖拽、点击选择目标、技能释放目标选择等核心输入逻辑
    /// 继承自单例自动挂载 MonoBehaviour 基类，保证全局唯一且自动初始化
    /// </summary>
    public class BattleInputHandler : SingletonAutoMono<BattleInputHandler>, IBattleInputHandler
    {
        // 拖拽起始位置（屏幕坐标）
        private Vector3 _dragStartPosition;
        // 是否处于拖拽状态（用于区分点击和拖拽行为）
        private bool _isDragging;
        // 拖拽阈值（超过该距离判定为拖拽，否则为点击）
        private const float dragThreshold = 110f;
        // 当前选中的技能ID（用于释放技能时匹配技能配置）
        private int skillId;
        private IBattleContext _context;
        private Camera _camera;
        private Action _OnLeftDrag;
        private Action _OnRightDrag;
        private Action<IBattleEntityObject> _OnSelectedObject;

        /// <summary>
        /// 选中战斗实体对象的事件（如选中玩家/怪物作为技能目标）
        /// 事件参数：选中的战斗实体对象接口
        /// </summary>
        public event Action<IBattleEntityObject> OnSelectedObject
        {
            add
            {
                if (_OnSelectedObject != null)
                {
                    LogManager.LogError($"{nameof(OnSelectedObject)}重复添加");
                    return;
                }
                _OnSelectedObject += value;
            }
            remove => _OnSelectedObject -= value;
        }
        
        /// <summary>
        /// 向左拖拽的事件（用于切换目标等逻辑）
        /// </summary>
        public event Action OnLeftDrag
        {
            add
            {
                if (_OnLeftDrag != null)
                {
                    LogManager.LogError($"{nameof(OnLeftDrag)}重复添加");
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
                    LogManager.LogError($"{nameof(OnRightDrag)}重复添加");
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

        public GameObject GameObject { get; private set; }
        
        /// <summary>
        /// 初始化方法
        /// 注册更新监听、订阅技能选择事件
        /// </summary>
        private void Awake()
        {
            GameObject = gameObject;
            _context = ServiceLocator.Get<IBattleManager>().GetContext();
        }
        
        private void Start()
        {
            _camera = Camera.main;
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context"></param>
        public void Init(IBattleContext context)
        {
            // 注册帧更新监听，每帧执行输入处理逻辑
            ServiceLocator.Get<IMonoAdapter>().AddUpdateListener(OnUpdate);
            // 从战斗管理器事件总线订阅技能选择事件，接收选中的技能ID
            context.GetEventBus().AddListener<SelectSkillEvent>(OnSelectSkillEvent);
        }

        /// <summary>
        /// 技能选择事件回调
        /// 接收并缓存选中的技能ID
        /// </summary>
        /// <param name="selectSkillEvent">技能选择事件数据</param>
        private void OnSelectSkillEvent(SelectSkillEvent selectSkillEvent)
        {
            skillId = selectSkillEvent.SkillId;
        }

        /// <summary>
        /// 帧更新回调方法
        /// 统一调度输入处理逻辑
        /// </summary>
        private void OnUpdate()
        {
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
                _dragStartPosition = UnityEngine.Input.mousePosition;
            }
            
            if (Mouse.current.leftButton.isPressed)
            {
                // 未进入拖拽状态时，判断鼠标移动距离是否超过阈值，超过则标记为拖拽状态
                if (!_isDragging && Vector2.Distance(UnityEngine.Input.mousePosition, _dragStartPosition) > dragThreshold)
                {
                    _isDragging = true;
                }

                // 处于拖拽状态时，处理拖拽偏移逻辑
                if (_isDragging)
                {
                    // 计算当前鼠标位置与起始位置的X轴偏移量
                    var dragDeltaX = UnityEngine.Input.mousePosition.x - _dragStartPosition.x;
                    // 触发拖拽中事件，传递X轴偏移量
                    OnDrag?.Invoke(dragDeltaX);

                    // 偏移量超过阈值时，触发左右拖拽事件并重置起始位置（避免重复触发）
                    if (Mathf.Abs(dragDeltaX) > dragThreshold)
                    {
                        // 向右拖拽：触发右拖拽事件
                        if (dragDeltaX > 0)
                        {
                            _OnRightDrag?.Invoke();
                        }
                        // 向左拖拽：触发左拖拽事件
                        else if (dragDeltaX < 0)
                        {
                            _OnLeftDrag?.Invoke();
                        }
                        // 重置拖拽起始位置，用于后续继续拖拽的偏移计算
                        _dragStartPosition = UnityEngine.Input.mousePosition;
                    }
                }
            }
            
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                // 若是拖曳中释放鼠标，则不处理
                if (_isDragging)
                {
                    // 重置拖拽状态，结束本次拖拽
                    _isDragging = false;
                    return;
                }
                
                // 校验技能ID有效性（避免空引用）
                if (!ServiceLocator.Get<IBinaryDataManager>().GetConfig<SkillInfoContainer>(EConfigLoadType.Excel).dataDic.ContainsKey(skillId))
                {
                    LogManager.LogWarning($"技能ID {skillId} 不存在，无法获取技能配置");
                    return;
                }

                // 根据选中的技能ID获取技能配置信息
                var skillInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<SkillInfoContainer>(EConfigLoadType.Excel).dataDic[skillId];
                // 将技能范围类型转换为技能目标类型（友方/敌方）
                var targetType = (E_SkillTargetType)skillInfo.f_SkillTargetType;

                // 根据技能目标类型设置射线检测的层级掩码（只检测对应层级的对象）
                int layerMask;
                switch (targetType)
                {
                    case E_SkillTargetType.Friend:
                        // 检测玩家对象层级
                        layerMask = LayerGeter.GetRoleBitLayer();
                        break;
                    case E_SkillTargetType.Enemy:
                        // 检测怪物对象层级
                        layerMask = LayerGeter.GetMonsterBitLayer();
                        break;
                    default:
                        LogManager.LogWarning($"未处理的技能目标类型：{targetType}");
                        return;
                }
                
                // 从鼠标屏幕位置发射射线，检测对应层级的战斗对象
                if (Physics.Raycast(_context.GetProxy().CurrentActiveCamera.ScreenPointToRay(UnityEngine.Input.mousePosition), out var hitInfo, 500, layerMask))
                {
                    // 获取射线命中对象挂载的战斗对象组件
                    var currentMainTarget = hitInfo.collider.GetComponent<BattleObject>();
                    if (currentMainTarget)
                    {
                        // 触发选中对象事件，传递选中的战斗对象
                        _OnSelectedObject?.Invoke(currentMainTarget);
                        LogManager.Log($"选中技能目标：{currentMainTarget.name}");
                    }
                    else
                    {
                        LogManager.LogWarning("射线命中对象未挂载BattleObject组件");
                    }
                }
            }
        }

        /// <summary>
        /// 组件禁用时的清理逻辑
        /// 移除更新监听、清空事件委托（避免内存泄漏）
        /// </summary>
        private void OnDisable()
        {
            // 移除帧更新监听
            ServiceLocator.Get<IMonoAdapter>().RemoveUpdateListener(OnUpdate);
            // 清空事件委托，避免空引用和内存泄漏
            _OnSelectedObject = null;
            _OnLeftDrag = null;
            _OnRightDrag = null;
        }
    }
}