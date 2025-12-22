using System;

namespace Game.Battle
{
    /// <summary>
    /// 量子套4件套效果（实现套装效果接口，作为独立组件）
    /// </summary>
    public class QuantumRelicSetEffect : IRelicSetEffect
    {
        public string SetName { get; } = "量子之影";
        public int RequiredCount { get; } = 4;

        public IBattleEntityObject Owner { get; private set; }

        //public IEntityObject EntityObject { get; private set; }

        public IBattleEntityObject BattleEntity { get; private set; }

        IEntityObject IComponent.EntityObject { get; }

        private float _additionalDamageRatio = 0.5f; // 追加伤害倍率（配置表读取）

        void IComponent.Init(IEntityObject entityObject) { }

        public virtual void BattleInit(IBattleEntityObject battleEntity)
        {
            BattleEntity = battleEntity;
        }

        public void SetOwner(IBattleEntityObject owner)
        {
            Owner = owner;
        }

        public void Activate(IBattleEntityObject owner)
        {
            Console.WriteLine($"{owner.Name}激活{SetName}4件套效果！暴击后追加量子伤害");

            // 订阅“技能释放事件”（判断是否暴击，触发追加伤害）
            BattleEntity.Context.GetEventBus().AddListener<SkillCastEvent>(OnSkillCastHandler);

            // 辅助逻辑：2件套/4件套基础属性加成（直接调用角色属性API）
            var attributeBonus = RequiredCount switch
            {
                2 => new RelicEffect(E_RelicBoun.CriticalRate, 12),
                4 => new RelicEffect(E_RelicBoun.CriticalRate, 12), // 示例：4件套继承2件套效果
                _ => throw new NotImplementedException()
            };

            // owner.GetComponent<PropertyComponent>().AddRelicBonus(attributeBonus.RelicBoun, attributeBonus.BounValue);
        }

        private void OnSkillCastHandler(SkillCastEvent skillCastEvent)
        {
            // 触发条件：1. 是套装所有者释放技能 2. 技能触发暴击（简化：假设伤害>150判定为暴击）
            if (skillCastEvent.Caster != Owner || skillCastEvent.Damage <= 150)
            {
                return;
            }

            Console.WriteLine($"\n【遗器效果】{SetName}4件套触发！");

            int additionalDamage = (int)(skillCastEvent.Caster.GetComponent<PropertyComponent>().GetProperty<BattleProperty>().MaxAtk * _additionalDamageRatio);
            skillCastEvent.Targets[0].TakeDamage(additionalDamage, E_ElementType.Quantum);
            Console.WriteLine($"{skillCastEvent.Targets[0].Name}受到量子追加伤害：{additionalDamage}点");
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }
    }
}
