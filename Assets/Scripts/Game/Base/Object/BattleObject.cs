using Framework;
using Game.Battle;
using System.Collections;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 战斗对象
    /// </summary>
    public abstract class BattleObject : EntityObject, IBattleEntityObject
    {
        public IBattleContext Context { get; protected set; }

        public float ActionValue { get; protected set;  }

        public bool CanAct => actCount > 0;

        public int BattleEntityId { get; private set; }

        public GameObject SubGameObject { get; private set; }

        public int EntityPosIndex { get; set; }

        // 行动次数
        protected int actCount;

        public override void BaseInit(int id)
        {
            // 获取第一个子对象，通常带有Animator
            SubGameObject = this.GetComponentsInChildren<Transform>()[1].gameObject;
        }

        public virtual void BattleInit(int battleEntityId, IBattleContext context)
        {
            // 基础初始化
            BaseInit(battleEntityId);
            // 记录上下文
            Context = context;
            // 缓存战斗实体ID
            BattleEntityId = battleEntityId;
        }

        public int GetSpeed()
        {
            return GetComponent<PropertyComponent>().GetPropertyValue(E_DynamicPropertyType.CurrentSpeed);
        }

        public virtual void Heal(int value)
        {

        }

        /// <summary>
        /// 能否受伤
        /// </summary>
        /// <returns></returns>
        protected bool CanTakeDamage()
        {
            return true;
        }

        public void TakeDamage(DamageResult damageResult)
        {
            // 判断能否受伤
            if (!CanTakeDamage())
            {
                return;
            }

            // 受到伤害前执行
            OnPreTakeDamage(damageResult);
            // 受到伤害
            OnTakeDamage(damageResult);
        }

        /// <summary>
        /// 在受伤之前触发
        /// </summary>
        protected abstract void OnPreTakeDamage(DamageResult damageResult);

        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="damageResult"></param>
        protected virtual void OnTakeDamage(DamageResult damageResult)
        {
            // 播放受击动画
            this.GetComponent<BattleAnimationComponent>().SetAnimationState(E_AnimationType.Hit);

            PropertyComponent propertyComponent = this.GetComponent<PropertyComponent>();
            // 更新当前血量
            int currentHp = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentHp);
            propertyComponent.SetPropertyValue(E_DynamicPropertyType.CurrentHp, currentHp - damageResult.FinalDamage);

            // 若小于0，则等于0，死亡
            currentHp = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentHp);
            if (currentHp <= 0)
            {
                propertyComponent.SetPropertyValue(E_DynamicPropertyType.CurrentHp, 0);
                Die();
            }
        }

        public virtual void Die()
        {
            // 从上下文中移除
            Context.GetTurnManager().RemoveEntity(this);
            // 之后播放死亡动画
            this.GetComponent<AnimationComponent>().SetAnimationState(E_AnimationType.Death);
        }

        /// <summary>
        /// 执行行动
        /// </summary>
        public void ExecuteAction()
        {
            OnTurnStart();
            MonoManager.Instance.StartCoroutine(OnExceuteAction());
        }

        /// <summary>
        /// 在执行行动时调用
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerator OnExceuteAction();

        public void SetActionValue(float actionValue)
        {
            this.ActionValue = actionValue;
        }

        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="triggerSkillEvent"></param>
        protected virtual void CastSkill(int skillId)
        {
            this.GetComponent<SkillComponent>().CastSkill(skillId);
        }

        /// <summary>
        /// 在回合开始时调用
        /// </summary>
        protected virtual void OnTurnStart()
        {
            AddActCount();
        }

        /// <summary>
        /// 在回合结束时调用
        /// </summary>
        protected virtual void OnTurnEnd()
        {

        }

        public void EnableAct()
        {

        }

        public void DisableAct()
        {

        }

        public void AddActCount()
        {
            ++actCount;
            if (actCount > 0)
            {
                // 执行实体回合开始事件
                Context.GetEventBus().TriggerEvent(new TurnStartEvent(Context, this));
            }
        }

        public void SubActCount()
        {
            actCount = Mathf.Clamp(--actCount, 0, actCount);
            if (actCount <= 0)
            {
                OnTurnEnd();
                // 执行实体回合结束事件
                Context.GetEventBus().TriggerEvent(new TurnEndEvent(Context, this));
            }
        }
    }
}
