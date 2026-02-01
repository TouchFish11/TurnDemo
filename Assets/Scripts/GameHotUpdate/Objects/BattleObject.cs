using System.Collections;
using Game.Animation;
using Game.Battle.Context;
using Game.Battle.Damage;
using Game.Battle.Enum;
using Game.Battle.Objects;
using Game.Battle.Skill.Component;
using Game.Objects;
using GameHotUpdate.Animation;
using GameHotUpdate.Battle.Event.Turn;
using GameHotUpdate.Property;
using UnityEngine;

namespace GameHotUpdate.Objects
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
        /// 是否可执行行动（行动次数>0时可行动）
        /// </summary>
        public bool CanAct => actCount > 0;

        /// <summary>
        /// 战斗实体唯一ID
        /// </summary>
        public int BattleEntityId { get; private set; }

        /// <summary>
        /// 战斗对象的子游戏物体（用于挂载动画、特效等组件）
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

        /// <summary>
        /// 剩余可行动次数（每回合初始为1，行动后减少）
        /// </summary>
        protected int actCount;

        /// <summary>
        /// 基础初始化方法
        /// </summary>
        /// <param name="id">战斗实体ID</param>
        public override void BaseInit(int id)
        {
            // 获取第二个子物体作为子游戏物体（默认第一个是自身，第二个为可视化表现层）
            // 用于绑定Animator等战斗相关组件
            SubGameObject = GetComponentsInChildren<Transform>()[1].gameObject;
        }

        /// <summary>
        /// 战斗初始化方法（战斗开始时调用）
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

        /// <summary>
        /// 获取当前速度属性值
        /// </summary>
        /// <returns>当前速度数值</returns>
        public int GetSpeed()
        {
            return GetComponent<PropertyComponent>().GetPropertyValue(E_DynamicPropertyType.CurrentSpeed);
        }

        /// <summary>
        /// 治疗方法（恢复血量，子类可重写实现具体逻辑）
        /// </summary>
        /// <param name="value">治疗数值</param>
        public virtual void Heal(int value)
        {

        }

        /// <summary>
        /// 判定是否可承受伤害
        /// （如无敌、免疫状态下返回false）
        /// </summary>
        /// <returns>true=可承受伤害，false=不可承受伤害</returns>
        protected bool CanTakeDamage()
        {
            return true;
        }

        /// <summary>
        /// 承受伤害入口方法
        /// </summary>
        /// <param name="damageResult">伤害结算结果对象（包含最终伤害、伤害类型等信息）</param>
        public bool TryTakeDamage(DamageResult damageResult)
        {
            // 判定是否可承受伤害，不可承受则直接返回
            if (!CanTakeDamage())
            {
                return false;
            }

            // 承受伤害前的预处理（抽象方法，子类实现）
            OnPreTakeDamage(damageResult);
            // 执行承受伤害核心逻辑
            OnTakeDamage(damageResult);
            
            return true;
        }

        /// <summary>
        /// 承受伤害前的预处理（抽象方法）
        /// 子类需实现伤害减免、护盾抵消、伤害反射等前置逻辑
        /// </summary>
        /// <param name="damageResult">伤害结算结果对象</param>
        protected abstract void OnPreTakeDamage(DamageResult damageResult);

        /// <summary>
        /// 承受伤害核心逻辑
        /// 处理扣血、播放受击动画等基础逻辑，子类可重写扩展
        /// </summary>
        /// <param name="damageResult">伤害结算结果对象</param>
        protected virtual void OnTakeDamage(DamageResult damageResult)
        {
            // 播放受击动画
            GetComponent<BattleAnimationComponent>().SetAnimationState(E_AnimationType.Hit);

            // 获取属性组件，处理血量扣减
            var propertyComponent = GetComponent<PropertyComponent>();
            // 获取当前血量
            var currentHp = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentHp);
            // 扣减最终伤害量
            propertyComponent.SetPropertyValue(E_DynamicPropertyType.CurrentHp, currentHp - damageResult.FinalDamage);

            // 修正血量：最小为0（防止血量为负数）
            currentHp = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentHp);
            if (currentHp <= 0)
            {
                propertyComponent.SetPropertyValue(E_DynamicPropertyType.CurrentHp, 0);
            }
        }

        /// <summary>
        /// 死亡逻辑（抽象方法）
        /// 子类需实现死亡动画、掉落、移除战斗等具体逻辑
        /// </summary>
        /// <returns>协程迭代器（用于处理异步死亡流程）</returns>
        public abstract IEnumerator Die();

        /// <summary>
        /// 执行行动入口方法（回合开始时调用）
        /// </summary>
        public void ExecuteAction()
        {
            // 触发回合开始逻辑
            OnTurnStart();
            // 若可行动则执行具体行动逻辑
            if (CanAct)
            {
                StartCoroutine(OnExceuteAction());
            }
        }

        /// <summary>
        /// 执行具体行动逻辑（抽象方法）
        /// 子类需实现技能释放、普通攻击、移动等核心行动逻辑
        /// </summary>
        /// <returns>协程迭代器（用于处理异步行动流程）</returns>
        protected abstract IEnumerator OnExceuteAction();

        /// <summary>
        /// 设置行动值
        /// </summary>
        /// <param name="actionValue">新的行动值</param>
        public void SetActionValue(float actionValue)
        {
            ActionValue = actionValue;
        }

        /// <summary>
        /// 释放技能（虚方法，子类可重写扩展释放规则）
        /// </summary>
        /// <param name="skillId">技能ID</param>
        protected virtual void CastSkill(int skillId)
        {
            // 调用技能组件释放指定ID的技能
            GetComponent<SkillComponent>().CastSkill(skillId);
        }

        /// <summary>
        /// 回合开始时的逻辑处理
        /// 触发回合开始事件、初始化行动次数等
        /// </summary>
        protected virtual void OnTurnStart()
        {
            // 触发回合开始事件（供外部监听）
            Context.GetEventBus().TriggerEvent(new TurnStartEvent(Context, this));
            // 增加行动次数（默认每回合初始可行动1次）
            AddActCount();
        }

        /// <summary>
        /// 回合结束时的逻辑处理（虚方法，子类可扩展）
        /// </summary>
        protected virtual void OnTurnEnd()
        {

        }

        /// <summary>
        /// 启用行动能力（预留方法，用于解除行动限制）
        /// </summary>
        public void EnableAct()
        {

        }

        /// <summary>
        /// 禁用行动能力（预留方法，用于施加行动限制）
        /// </summary>
        public void DisableAct()
        {

        }

        /// <summary>
        /// 增加可行动次数
        /// </summary>
        public void AddActCount()
        {
            ++actCount;
        }

        /// <summary>
        /// 减少可行动次数
        /// 次数≤0时触发回合结束逻辑
        /// </summary>
        public void SubActCount()
        {
            // 减少行动次数并限制最小值为0
            actCount = Mathf.Clamp(--actCount, 0, actCount);
            // 行动次数耗尽时触发回合结束
            if (actCount > 0)
            {
                return;
            }
            
            OnTurnEnd();
            // 触发回合结束事件（供外部监听）
            Context.GetEventBus().TriggerEvent(new TurnEndEvent(Context, this));
        }
    }
}