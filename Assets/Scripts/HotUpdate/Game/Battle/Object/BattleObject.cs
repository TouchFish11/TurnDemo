using System.Collections;
using System.Collections.Generic;
using HotUpdate.Base.ECModule;
using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Damage;
using HotUpdate.Game.Battle.Event.General;
using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.Object.Conditions;
using HotUpdate.Game.Battle.ResponsibilityChain;
using HotUpdate.Game.Battle.Skill.Conditions;
using HotUpdate.Game.Battle.Skill.Factory;
using HotUpdate.Game.Battle.StatSystem;
using HotUpdate.Game.Battle.TargetSelect;
using HotUpdate.Game.Battle.UI;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object
{
    /// <summary>
    /// 战斗对象基类
    /// 所有参与战斗的实体（角色、怪物、NPC等）的抽象基类，实现了战斗实体核心接口，定义战斗行为规范
    /// </summary>
    public abstract class BattleObject : EntityObject, IBattleEntityObject, IDisplayPendingExecution
    {
        private bool _hasAction;            // 行动预算：本回合是否还有行动次数
        private bool _actable;              // 行动资格：是否被眩晕/死亡剥夺
        private bool _acting;               // 是否正在演出技能
        private bool _pendingExtraTurn;     // 是否有额外回合

        protected ICastSkillConditionFactory castSkillConditionFactory; // 技能释放条件工厂
        protected ITargetSelectStrategyFactory targetSelectStrategyFactory; // 目标选择策略工厂
        protected Commandfactory commandfactory; // 命令工厂
        protected IDeathHandler deathHandler; // 死亡处理器
        protected Handler<DamageResult> damageChain; // 伤害处理
        protected List<IDeathCondition> _deathConditions; // 死亡条件缓存
        
        public bool CanAct => _hasAction && _actable;
        public bool Acting => _acting;
        public bool TurnFinished => !CanAct && !_acting;
        public ITurnActionDriver TurnActionDriver { get; private set; }
        public IBattleContext Context { get; protected set; }
        public float ActionValue { get; set; }
        public int BattleEntityId { get; private set; }
        public GameObject SubGameObject { get; private set; }
        public int EntityPosIndex { get; set; }
        public abstract ISkillFactory SkillFactory { get; protected set; }
        public abstract ICastSkillCondition DefaultCastCondition { get; protected set; }
        public abstract ITargetSelectStrategy DefaultTargetSelectStrategy { get; protected set; }
        public IBattleEntityObject BattleEntity => this;
        public bool IsDead => _deathConditions.FindIndex(c => !c.CanDie(this)) == -1;

        protected override void OnInit()
        {
            // 获取第二个子物体作为子游戏物体（默认第一个是自身，第二个为可视化表现层），用于绑定Animator等战斗相关组件
            SubGameObject = GetComponentsInChildren<Transform>()[1].gameObject;
        }

        /// <summary>
        /// 战斗初始化方法
        /// </summary>
        /// <param name="parameter"></param>
        protected void BattleInit(BattleParameterObject parameter)
        {
            Context = parameter.BattleContext;
            BattleEntityId = parameter.BattleEntityId;
            _deathConditions = parameter.DeathConditions;
            commandfactory = parameter.Commandfactory;
            castSkillConditionFactory = parameter.CastSkillConditionFactory;
            targetSelectStrategyFactory = parameter.TargetSelectStrategyFactory;
            deathHandler = parameter.DeathHandler;
            TurnActionDriver = parameter.TurnActionDriver;
            SkillFactory = GetSkillFactory();
            DefaultTargetSelectStrategy = GetTargetSelectStrategy();
            DefaultCastCondition = GetSkillCondition();
        }

        /// <summary>
        /// 获取技能工厂
        /// </summary>
        /// <returns></returns>
        protected abstract ISkillFactory GetSkillFactory();

        /// <summary>
        /// 获取技能释放条件
        /// </summary>
        /// <returns></returns>
        protected abstract ICastSkillCondition GetSkillCondition();

        /// <summary>
        /// 获取目标选择策略
        /// </summary>
        /// <returns></returns>
        protected abstract ITargetSelectStrategy GetTargetSelectStrategy();
        
        /// <summary>
        /// 消耗行动预算：必须在 InsertCommandEvent 之前调用
        /// （事件会同步走到 BuildPendingDisplayList 读 CanAct）
        /// </summary>
        protected void ConsumeAction()
        {
            _hasAction = false;
        }
        
        // 标记演出中：命令插入之后调用
        protected void BeginActing()
        {
            _acting = true;
        }
        
        public void EndActing()
        {
            _acting = false;
        }

        public void DisableAction()
        {
            _actable = false;
        }

        public void ExecuteAction()
        {
            GrantTurn();

            var turnStartCommand = commandfactory.GetTurnStartCommand(this);
            Context.EventBus.TriggerEvent(new InsertCommandEvent(Context, turnStartCommand));
            return;
            
            // 授予回合
            void GrantTurn()
            {
                _hasAction = true;
                _actable = true;
                _acting = false;
            }
        }
        
        /// <summary>
        /// 额外回合用：重新授予行动预算（不重新结算）
        /// </summary>
        public void GrantAction()
        {
            _hasAction = true;
        }
        
        /// <summary>
        /// 额外回合判定：技能释放期间置位（如击杀触发），后处理时消费
        /// </summary>
        public void MarkExtraTurn()
        {
            _pendingExtraTurn = true;
        }
        
        /// <summary>
        /// 尝试消耗额外回合
        /// </summary>
        /// <returns></returns>
        public bool TryConsumeExtraTurn()
        {
            var v = _pendingExtraTurn; 
            _pendingExtraTurn = false; 
            return v;
        }

        public void AddDeathCondition(IDeathCondition condition)
        {
            _deathConditions.Add(condition);
        }

        public bool RemoveDeathCondition(IDeathCondition condition)
        {
            return _deathConditions.Remove(condition);
        }

        public abstract void CastSkill(int skillId);

        public void TakeHeal(int healAmount)
        {
            var statComponent = GetComponent<StatsComponent>();
            statComponent.UpdateHealth(healAmount);
            // 触发应用治疗事件
            Context.EventBus.TriggerEvent(new ApplyHealEvent(Context, this, healAmount));
        }

        public void TakeSheild(int shieldAmount)
        {
            var statsComponent = GetComponent<StatsComponent>();
            statsComponent.UpdateShield(shieldAmount);
        }

        public void TakeDamage(DamageResult damageResult)
        {
            damageChain.HandleRequest(damageResult);
        }

        public IEnumerator Die()
        {
            yield return deathHandler.HandleDeath();
        }
    }
}