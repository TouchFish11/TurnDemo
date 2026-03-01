using Core.Pool;
using GameHotUpdate.Battle.Context;
using GameHotUpdate.Battle.Object;
using GameHotUpdate.Battle.Status.Data;

namespace GameHotUpdate.Battle.Status
{
    /// <summary>
    /// 状态基类：所有战斗状态（如buff/debuff）的父类，实现状态的基础生命周期和属性管理
    /// </summary>
    public abstract class Status : IStatus, IPoolData
    {
        // 状态是否有效（有效则生效，无效则触发移除逻辑）
        private bool _isValid;
        // 状态加成数据（如属性加成、数值变化等）
        protected StatusBonusData bonusData;

        /// <summary>
        /// 状态核心属性（包含状态ID、剩余回合、当前层数等）
        /// </summary>
        public StatusProperty StatusProperty { get; protected set; }

        /// <summary>
        /// 状态施加者（如释放技能的角色）
        /// </summary>
        public IBattleEntityObject Sourcer { get; private set; }

        /// <summary>
        /// 状态拥有者（如被施加buff的角色）
        /// </summary>
        public IBattleEntityObject Owner { get; private set; }

        /// <summary>
        /// 只读获取状态加成数据
        /// </summary>
        public StatusBonusData BonusData => bonusData;

        /// <summary>
        /// 状态有效性标识：赋值时自动触发添加/移除逻辑
        /// </summary>
        public bool IsValid
        {
            get => _isValid;
            set
            {
                _isValid = value;
                if (value)
                {
                    OnAdd(); // 状态生效时执行添加逻辑
                }
                else
                {
                    OnRemove(); // 状态失效时执行移除逻辑
                }
            }
        }

        /// <summary>
        /// 初始化状态核心信息
        /// </summary>
        /// <param name="sorucer">施加者</param>
        /// <param name="owner">拥有者</param>
        /// <param name="statusId">状态配置ID</param>
        public void InitStatus(IBattleEntityObject sorucer, IBattleEntityObject owner, int statusId)
        {
            StatusProperty = new StatusProperty(statusId); // 初始化状态属性
            bonusData = new StatusBonusData(); // 初始化加成数据
            Sourcer = sorucer; // 赋值施加者
            Owner = owner; // 赋值拥有者
        }

        /// <summary>
        /// 调整状态层数
        /// </summary>
        /// <param name="deltaPine">层数变化量（正数加层，负数减层）</param>
        public void ChangePine(int deltaPine)
        {
            // 更新当前层数
            StatusProperty.SetCurrentPine(StatusProperty.CurrentPine + deltaPine);
            // 触发层数变化回调
            OnPineChanged();
        }

        /// <summary>
        /// 回合开始时的状态处理（外部调用入口）
        /// </summary>
        /// <param name="owner">状态拥有者</param>
        /// <param name="context">战斗上下文（包含战斗环境、规则等信息）</param>
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
        /// 不同状态在回合开始时有不同行为（如持续掉血、回蓝等）
        /// </summary>
        /// <param name="owner">状态拥有者</param>
        /// <param name="context">战斗上下文</param>
        protected abstract void OnTurnStart(IBattleEntityObject owner, IBattleContext context);

        /// <summary>
        /// 回合结束时的自定义逻辑（子类可选重写）
        /// </summary>
        /// <param name="owner">状态拥有者</param>
        /// <param name="context">战斗上下文</param>
        protected virtual void OnTurnEnd(IBattleEntityObject owner, IBattleContext context) { }

        /// <summary>
        /// 重置状态数据（对象池回收时调用）
        /// 清空所有引用和状态标识，避免内存泄漏
        /// </summary>
        public void ResetData()
        {
            _isValid = false;
            StatusProperty = null;
            Sourcer = null;
            Owner = null;
        }
    }
}