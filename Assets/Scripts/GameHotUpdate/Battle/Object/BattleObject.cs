using System.Collections;
using Game.Battle.Context;
using Game.Battle.Damage;
using Game.Battle.Enum;
using Game.Battle.Objects;
using Game.Objects;
using GameHotUpdate.Battle.Event.General;
using GameHotUpdate.Battle.Property;
using GameHotUpdate.Battle.ResponsibilityChain;
using UnityEngine;

namespace GameHotUpdate.Battle.Object
{
    /// <summary>
    /// 战斗对象基类
    /// 所有参与战斗的实体（角色、怪物、NPC等）的抽象基类，实现了战斗实体核心接口，定义战斗行为规范
    /// </summary>
    public abstract class BattleObject : EntityObject, IBattleEntityObject, IDamagable
    {
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

        /// <summary>
        /// 是否死亡（当前血量≤0判定为死亡）
        /// </summary>
        public bool IsDead => GetComponent<PropertyComponent>().GetPropertyValue(E_DynamicPropertyType.CurrentHp) <= 0;

        // 伤害处理链
        protected Handler<DamageResult> damageChain;

        /// <summary>
        /// 基础初始化方法
        /// </summary>
        /// <param name="id">战斗实体ID</param>
        public override void BaseInit(int id)
        {
            // 获取第二个子物体作为子游戏物体（默认第一个是自身，第二个为可视化表现层），用于绑定Animator等战斗相关组件
            SubGameObject = GetComponentsInChildren<Transform>()[1].gameObject;
        }

        /// <summary>
        /// 战斗初始化方法
        /// </summary>
        /// <param name="battleEntityId">战斗实体唯一ID</param>
        /// <param name="context">战斗上下文实例</param>
        public virtual void BattleInit(int battleEntityId, IBattleContext context)
        {
            // 执行基础初始化
            BaseInit(battleEntityId);
            // 绑定战斗上下文
            Context = context;
            // 赋值战斗实体ID
            BattleEntityId = battleEntityId;
        }

        public virtual void ExecuteAction()
        {
            // 重置标志
            CanAct = true;
        }
        
        public void TakeHeal(int healAmount)
        {
            var propertyComponent = this.GetComponent<PropertyComponent>();
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
            var propertyComponent = this.GetComponent<PropertyComponent>();
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
        
        public abstract IEnumerator Die();
        
        public void SetActionValue(float actionValue)
        {
            ActionValue = actionValue;
        }
    }
}