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

        // 行动次数
        protected int actCount;

        public virtual void BattleInit(int battleEntityId, IBattleContext context)
        {
            // 基础初始化
            BaseInit(battleEntityId);
            // 记录上下文
            Context = context;
            // 缓存战斗实体ID
            BattleEntityId = battleEntityId;

            //// 敌人角色额外加载韧性组件（示例：弱点属性=物理，初始韧性=200）
            //if (name.Contains("敌人"))
            //{
            //    ToughnessComponent toughnessComponent = this.AddComponent<ToughnessComponent>();
            //    toughnessComponent.Init(this, new() { E_ElementType.Physical }, 200);
            //}
        }

        public int GetSpeed()
        {
            return GetComponent<PropertyComponent>().GetPropertyValue(E_DynamicPropertyType.CurrentSpeed);
        }

        public virtual void Heal(int value)
        {

        }

        public virtual void TakeDamage(DamageResult damageResult)
        {
            // 需要判断能否受伤
            // ...

            // 先削减韧性
            ToughnessComponent toughnessComponent = this.GetComponent<ToughnessComponent>();
            toughnessComponent.ReduceToughness(damageResult.Source, damageResult.ElementType, damageResult.ToughenValue);

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
            // TODO：待优化，目前直接失活对象。之后播放死亡动画
            //LogManager.Log($"实体：{this.GameObject.name}死亡");
            // this.GameObject.SetActive(false);
        }

        /// <summary>
        /// 执行行动
        /// </summary>
        public void ExecuteAction()
        {
            AddActCount();
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
           // LogManager.Log($"行动数增加，{gameObject.name}剩余行动次数：{actCount}");
        }

        public void SubActCount()
        {
            actCount = Mathf.Clamp(--actCount, 0, actCount);
            if (actCount <= 0)
            {
                // 执行实体回合结束事件
                Context.GetEventBus().TriggerEvent(new TurnEndEvent(Context, this, false));
            }
        }
    }
}
