using System;
using System.Collections;
using System.Collections.Generic;
using Core.DI;
using Core.Exceptions;
using HotUpdate.Base.ECModule;
using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Damage;
using HotUpdate.Game.Battle.Event.General;
using HotUpdate.Game.Battle.Object.Conditions;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Battle.Object.StateMeachine;
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
    public abstract class BattleObject : EntityObject, IBattleEntityObject, IDamagable, IDisplayPendingExecution
    {
        // 技能释放条件工厂
        protected ICastSkillConditionFactory castSkillConditionFactory;
        // 目标选择策略工厂
        protected ITargetSelectStrategyFactory targetSelectStrategyFactory;
        // 命令工厂
        protected Commandfactory commandfactory;
        // 死亡处理器
        protected IDeathHandler deathHandler;
        // 伤害处理链
        protected Handler<DamageResult> damageChain;
        // 角色回合阶段状态缓存
        private readonly Dictionary<EActPhase, ITurnState> _turnStates = new();
        // 当前实体所处的行动状态
        private ITurnState _currentState;
        // 死亡条件缓存
        protected List<IDeathCondition> _deathConditions;
        // 回合操作驱动对象
        private ITurnActionDriver _turnActionDriver;
        
        public EActPhase CurrentActPhase { get; set; }
        
        public bool Acting { get; set; }
        
        public IBattleContext Context { get; protected set; }
        
        public float ActionValue { get; set; }
        
        public bool CanAct { get; set; }
        
        public int BattleEntityId { get; private set; }
        
        public GameObject SubGameObject { get; private set; }
        
        public int EntityPosIndex { get; set; }

        public abstract ISkillFactory SkillFactory { get; protected set; }
        
        public abstract ICastSkillCondition DefaultCastCondition { get; protected set;}
        
        public abstract ITargetSelectStrategy DefaultTargetSelectStrategy { get; protected set;}
        
        public IBattleEntityObject BattleEntity => this;
        
        public bool IsDead
        {
            get
            {
                var isDead = true;
                // 所有死亡条件都满足时才能死亡
                foreach (var condition in _deathConditions)
                {
                    if (!condition.CanDie(this))
                    {
                        isDead = false;
                        break;
                    }
                }

                return isDead;
            }
        }

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
            // 初始化依赖
            CurrentActPhase = EActPhase.TurnStart;
            Context = parameter.BattleContext;
            BattleEntityId = parameter.BattleEntityId;
            _deathConditions = parameter.DeathConditions;
            commandfactory = parameter.Commandfactory;
            castSkillConditionFactory = parameter.CastSkillConditionFactory;
            targetSelectStrategyFactory = parameter.TargetSelectStrategyFactory;
            deathHandler = parameter.DeathHandler;
            _turnActionDriver = parameter.TurnActionDriver;
            SkillFactory = GetSkillFactory();
            DefaultTargetSelectStrategy = GetTargetSelectStrategy();
            DefaultCastCondition = GetSkillCondition();
            AddState(EActPhase.TurnStart);
            AddState(EActPhase.Executing);
            AddState(EActPhase.TurnEnd);
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
        /// 回合开始步骤逻辑节点列表
        /// </summary>
        /// <returns></returns>
        protected abstract List<ITurnStartNode> GetStartNodes();
        
        /// <summary>
        /// 添加状态方法
        /// </summary>
        /// <param name="phase"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected void AddState(EActPhase phase)
        {
            switch (phase)
            {
                case EActPhase.TurnStart:
                    _turnStates.TryAdd(EActPhase.TurnStart, DIContainer.Create<TurnStartState>(this, GetStartNodes()));
                    break;
                case EActPhase.Executing:
                    _turnStates.TryAdd(EActPhase.Executing, DIContainer.Create<TurnExecutingState>(this, _turnActionDriver));
                    break;
                case EActPhase.TurnEnd:
                    _turnStates.TryAdd(EActPhase.TurnEnd, DIContainer.Create<TurnEndState>(this));
                    break;
                case EActPhase.None:
                default:
                    throw ExceptionHelper.Throw<ArgumentOutOfRangeException>($"{nameof(EActPhase)}:{phase}");
            }
        }
        
        public void ChangeState(EActPhase eActPhase)
        {
            _currentState?.Exit();
            _currentState = _turnStates[eActPhase];
            _currentState.Enter();
        }

        public void ExecuteAction()
        {
            // 重置行动标志
            CanAct = true;
            Acting = false;
            ChangeState(EActPhase.TurnStart);
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