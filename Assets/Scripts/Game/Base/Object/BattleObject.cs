using Framework;
using Game.Battle;
using GameLogic.BattleMoudule.Entity;
using System.Collections;
using System.Collections.Generic;

namespace Game
{
    /// <summary>
    /// 战斗对象
    /// </summary>
    public abstract class BattleObject : EntityObject, IBattleEntityObject
    {
        public string Name { get; protected set; }

        public IBattleContext Context { get; protected set; }

        // 基础属性
        private readonly Dictionary<E_FieldType, float> _attributes = new Dictionary<E_FieldType, float>();
        // 额外属性加成
        private readonly Dictionary<E_FieldType, float> _attributeBonuses = new Dictionary<E_FieldType, float>();

        public virtual void BattleInit(int roleId, IBattleContext context)
        {
            // 记录上下文
            Context = context;

            // 加载组件（配置表可配置角色绑定哪些组件）
            SkillComponent skillComponent = this.AddComponent<SkillComponent>();
            skillComponent.BattleInit(this);

            TalentComponent talentComponent = this.AddComponent<TalentComponent>();
            talentComponent.BattleInit(this);

            RelicComponent relicComponent = this.AddComponent<RelicComponent>();
            relicComponent.BattleInit(this);

            AdditionalAttackComponent additionalAttackComponent = this.AddComponent<AdditionalAttackComponent>();
            additionalAttackComponent.BattleInit(this);

            SummonComponent summonComponent =   this.AddComponent<SummonComponent>();
            summonComponent.BattleInit(this);

            // 敌人角色额外加载韧性组件（示例：弱点属性=物理，初始韧性=200）
            if (name.Contains("敌人"))
            {
                ToughnessComponent toughnessComponent = this.AddComponent<ToughnessComponent>();
                toughnessComponent.Init(this, new() { E_PropertyType.Physical }, 200);
            }
        }

        public virtual void AddRelicBonus(E_RelicBoun type, float value)
        {
            E_FieldType fieldType = E_FieldType.None;
            switch (type)
            {
                case E_RelicBoun.CriticalRate:
                    fieldType = E_FieldType.CriticalRate;
                    break;
                case E_RelicBoun.CriticalDmg:
                    fieldType = E_FieldType.CriticalDmg;
                    break;
                case E_RelicBoun.BuildHp:
                    fieldType = E_FieldType.MaxHp;
                    break;
                case E_RelicBoun.Speed:
                    fieldType = E_FieldType.Speed;
                    break;
            }

            if (_attributeBonuses.ContainsKey(fieldType))
            {
                _attributeBonuses[fieldType] += value;
            }
            else
            {
                _attributeBonuses[fieldType] = value;
            }
        }

        public virtual int GetField(E_FieldType propertyType)
        {
            _attributes.TryGetValue(propertyType, out var baseValue);
            _attributeBonuses.TryGetValue(propertyType, out var bonusValue);
            return (int)(baseValue + bonusValue);
        }

        public abstract int GetSpeed();

        public virtual void Heal(int value)
        {

        }

        public virtual void TakeDamage(int damage, E_PropertyType propertyType)
        {
            LogManager.Log($"{Name}剩余HP：{1000 - damage}");
        }

        public abstract IEnumerator ExecuteAction();
    }
}
