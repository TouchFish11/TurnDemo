using Core.DI;
using Core.Pool;
using Core.Time;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.StatSystem;
using HotUpdate.Game.Battle.StatSystem.Modifiers;
using HotUpdate.Game.VFX;

namespace HotUpdate.Game.Battle.Statuses
{
    /// <summary>
    /// 状态基类：所有战斗状态（如buff/debuff）的父类，实现状态的基础生命周期和属性管理
    /// </summary>
    public abstract class StatusBase : IStatus
    {
        // 回收时不置空，否则复用会出问题
        [Inject] protected IPoolManager poolManager;
        [Inject] protected StatModifierFactory modifierFactory;
        [Inject] protected IVFXManager vfxManager;
        [Inject] protected ITimerManager timerManager;
        
        // 状态是否有效（有效则生效，无效则触发移除逻辑）
        private bool _isValid;
        
        public StatusInfo StatusInfo { get; private set; }
        
        public StatusProperty StatusProperty { get; protected set; }
        
        public IBattleEntityObject Sourcer { get; private set; }
        
        public IBattleEntityObject Owner { get; private set; }
        
        public EStatusState StatusState { get; private set; }
        
        public bool IsValid
        {
            get => _isValid;
            set
            {
                if (value && !_isValid)
                {
                    _isValid = true;
                    OnAdd(); // 状态生效时执行添加逻辑
                }
                else if(!value && _isValid)
                {
                    _isValid = false;
                    OnRemove(); // 状态失效时执行移除逻辑
                }
            }
        }

        /// <summary>
        /// 战斗上下文
        /// </summary>
        protected IBattleContext Context { get; private set; }
        
        /// <summary>
        /// 状态施加者的属性组件
        /// </summary>
        protected StatsComponent SourcerStatsComponent => Sourcer.GetComponent<StatsComponent>();

        /// <summary>
        /// 状态拥有者的属性组件
        /// </summary>
        protected StatsComponent OwnerStatsComponent => Owner.GetComponent<StatsComponent>();
        
        public void InitStatus(IBattleEntityObject sorucer, IBattleEntityObject owner, StatusInfo statusInfo)
        {
            StatusProperty = new StatusProperty(statusInfo); // 初始化状态属性
            StatusInfo = statusInfo;
            Sourcer = sorucer; // 赋值施加者
            Owner = owner; // 赋值拥有者
            Context = owner.Context;
            StatusState = (EStatusState)statusInfo.f_statusType;
        }

        /// <summary>
        /// 调整状态层数
        /// </summary>
        /// <param name="deltaPine">层数变化量（正数加层，负数减层）</param>
        public void ChangePine(int deltaPine)
        {
            // 更新当前层数
            StatusProperty.CurrentPine += deltaPine;
            // 触发层数变化回调
            OnPineChanged();
        }

        public void EnableActive()
        {
            StatusState = EStatusState.Active;
        }

        /// <summary>
        /// 回合开始时的状态处理（外部调用入口）
        /// </summary>
        /// <param name="owner">状态拥有者</param>
        /// <param name="context">战斗上下文</param>
        public virtual void TurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            OnTurnStart(owner, context); // 执行子类自定义的回合开始逻辑
            // 判定剩余回合/层数是否满足生效条件，不满足则失效
            if (StatusProperty.RemainingRound <= 0 || StatusProperty.CurrentPine <= 0)
            {
                IsValid = false;
            }
        }

        /// <summary>
        /// 回合结束时的状态处理（外部调用入口）
        /// </summary>
        /// <param name="owner">状态拥有者</param>
        /// <param name="context">战斗上下文</param>
        public virtual void TurnEnd(IBattleEntityObject owner, IBattleContext context)
        {
            OnTurnEnd(owner, context); // 执行子类自定义的回合结束逻辑
            // 判定剩余回合/层数是否满足生效条件，不满足则失效
            if (StatusProperty.RemainingRound <= 0 || StatusProperty.CurrentPine <= 0)
            {
                IsValid = false;
            }
        }

        /// <summary>
        /// 状态添加时的逻辑（子类重写）
        /// 仅当IsValid设为true时触发
        /// </summary>
        protected virtual void OnAdd() { }

        /// <summary>
        /// 状态层数变化时的逻辑（子类重写）
        /// </summary>
        protected virtual void OnPineChanged() { }

        /// <summary>
        /// 状态移除时的逻辑（子类重写）
        /// 仅当IsValid设为false时触发
        /// </summary>
        protected virtual void OnRemove() { }

        /// <summary>
        /// 回合开始时的自定义逻辑（抽象方法，子类必须实现）
        /// 不同状态在回合开始时有不同行为
        /// </summary>
        /// <param name="owner">状态拥有者</param>
        /// <param name="context">战斗上下文</param>
        protected virtual void OnTurnStart(IBattleEntityObject owner, IBattleContext context) { }

        /// <summary>
        /// 回合结束时的自定义逻辑（子类可选重写）
        /// </summary>
        /// <param name="owner">状态拥有者</param>
        /// <param name="context">战斗上下文</param>
        protected virtual void OnTurnEnd(IBattleEntityObject owner, IBattleContext context) { }

        /// <summary>
        /// 重置状态数据
        /// </summary>
        public void ResetData()
        {
            StatusState = EStatusState.None;
            Context = null;
            _isValid = false;
            StatusInfo = null;
            StatusProperty = null;
            Sourcer = null;
            Owner = null;
        }
    }
}