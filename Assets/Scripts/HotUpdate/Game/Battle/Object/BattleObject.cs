using System.Collections;
using System.Collections.Generic;
using HotUpdate.Base.ECModule;
using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Damage;
using HotUpdate.Game.Battle.Event.General;
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
        
        /// <summary>
        /// 死亡条件缓存
        /// </summary>
        protected List<IDeathCondition> DeathConditions { get; private set; }
        
        public bool Acting { get; set; }

        /// <summary>
        /// 战斗上下文，提供战斗环境、事件总线、规则等核心战斗数据访问
        /// </summary>
        public IBattleContext Context { get; protected set; }

        /// <summary>
        /// 行动值（速度相关），用于判定回合行动顺序
        /// </summary>
        public float ActionValue { get; set; }

        /// <summary>
        /// 是否可执行行动
        /// </summary>
        public bool CanAct { get; set; }

        /// <summary>
        /// 战斗实体唯一ID
        /// </summary>
        public int BattleEntityId { get; private set; }

        /// <summary>
        /// 战斗对象的子游戏物体（用于挂载动画组件）
        /// </summary>
        public GameObject SubGameObject { get; private set; }

        /// <summary>
        /// 战斗实体在阵型中的位置索引
        /// </summary>
        public int EntityPosIndex { get; set; }

        public abstract ISkillFactory SkillFactory { get; protected set; }
        
        public abstract ICastSkillCondition DefaultCastCondition { get; protected set;}
        
        public abstract ITargetSelectStrategy DefaultTargetSelectStrategy { get; protected set;}

        public IBattleEntityObject BattleEntity => this;

        /// <summary>
        /// 是否死亡
        /// </summary>
        public bool IsDead
        {
            get
            {
                var isDead = true;
                // 所有死亡条件都满足时才能死亡
                foreach (var condition in DeathConditions)
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
            // 绑定战斗上下文
            Context = parameter.BattleContext;
            // 赋值战斗实体ID
            BattleEntityId = parameter.BattleEntityId;
            // 初始化其它依赖
            commandfactory = parameter.Commandfactory;
            castSkillConditionFactory = parameter.CastSkillConditionFactory;
            targetSelectStrategyFactory = parameter.TargetSelectStrategyFactory;
            deathHandler = parameter.DeathHandler;
            DeathConditions = parameter.DeathConditions;
            parameter.DeathHandler.InitEntity(this);
        }

        public void ExecuteAction()
        {
            // 重置行动标志
            CanAct = true;
            Acting = false;
            OnExecuteAction();
        }

        public void AddDeathCondition(IDeathCondition condition)
        {
            DeathConditions.Add(condition);
        }

        public bool RemoveDeathCondition(IDeathCondition condition)
        {
            return DeathConditions.Remove(condition);
        }
        
        public abstract void CastSkill(int skillId);

        /// <summary>
        /// 在行动时的执行逻辑
        /// </summary>
        protected abstract void OnExecuteAction();
        
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