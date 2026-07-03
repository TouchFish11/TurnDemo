using System.Collections;
using HotUpdate.Base;
using HotUpdate.Base.Object;
using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Damage;
using HotUpdate.Game.Battle.Event.General;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.Property;
using HotUpdate.Game.Battle.ResponsibilityChain;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Conditions;
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
        /// 战斗上下文，提供战斗环境、事件总线、规则等核心战斗数据访问
        /// </summary>
        public IBattleContext Context { get; protected set; }

        /// <summary>
        /// 行动值（速度相关），用于判定回合行动顺序
        /// </summary>
        public float ActionValue { get; protected set; }

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
        /// 是否死亡（当前血量≤0判定为死亡）
        /// </summary>
        public bool IsDead => GetComponent<PropertyComponent>().GetPropertyValue(E_DynamicPropertyType.CurrentHp) <= 0;
        
        protected override void OnInit()
        {
            // 获取第二个子物体作为子游戏物体（默认第一个是自身，第二个为可视化表现层），用于绑定Animator等战斗相关组件
            SubGameObject = GetComponentsInChildren<Transform>()[1].gameObject;
        }

        /// <summary>
        /// 战斗初始化方法
        /// </summary>
        /// <param name="initData"></param>
        protected void BattleInit(BattleObjectInitData initData)
        {
            // 绑定战斗上下文
            Context = initData.BattleContext;
            // 赋值战斗实体ID
            BattleEntityId = initData.BattleEntityId;
            // 初始化工厂
            commandfactory = initData.Commandfactory;
            castSkillConditionFactory = initData.CastSkillConditionFactory;
            targetSelectStrategyFactory = initData.TargetSelectStrategyFactory;
            // 初始化死亡处理器
            initData.DeathHandler.InitEntity(this);
            deathHandler = initData.DeathHandler;
        }

        public void ExecuteAction()
        {
            // 重置行动标志
            CanAct = true;
            OnExecuteAction();
        }

        /// <summary>
        /// 在行动时的执行逻辑
        /// </summary>
        protected abstract void OnExecuteAction();
        
        public void TakeHeal(int healAmount)
        {
            var propertyComponent = GetComponent<PropertyComponent>();
            var currentHp = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentHp);
            propertyComponent.SetPropertyValue(E_DynamicPropertyType.CurrentHp, currentHp + healAmount);
            // 触发应用治疗事件
            Context.GetEventBus().TriggerEvent(new ApplyHealEvent(Context, this, healAmount));
        }

        /// <summary>
        /// 提供护盾
        /// </summary>
        /// <param name="shieldAmount">护盾量</param>
        public void TakeSheild(int shieldAmount)
        {
            var propertyComponent = GetComponent<PropertyComponent>();
            var currentShield = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentShield);
            propertyComponent.SetPropertyValue(E_DynamicPropertyType.CurrentShield, currentShield + shieldAmount);
            // 触发应用护盾件
            Context.GetEventBus().TriggerEvent(new ApplyShieldEvent(Context, this, shieldAmount));
        }

        public abstract void CastSkill(int skillId);
        
        public void TakeDamage(DamageResult damageResult)
        {
            damageChain.HandleRequest(damageResult);
        }

        public IEnumerator Die()
        {
            // 触发实体死亡事件
            Context.GetEventBus().TriggerEvent(new EntityDeadEvent(Context, this));
            yield return deathHandler.HandleDeath();
        }
        
        public void SetActionValue(float actionValue)
        {
            ActionValue = actionValue;
        }
    }
}