using System.Collections;

namespace Game.Battle
{
    /// <summary>
    /// 迷迷召唤物
    /// </summary>
    public class MimiSummon : BattleObject, ISummon
    {
        public IBattleEntityObject Owner { get; private set; }

        public void Init(IBattleEntityObject owner)
        {
            Owner = owner;
            // 订阅“主人技能释放事件”（主人放技能时，召唤物协同攻击）(可选)
            //BattleEventBus.AddListener<SkillCastEvent>(OnOwnerSkillCastHandler);
        }

        /// <summary>
        /// 事件回调：主人释放技能后，召唤物协同攻击
        /// </summary>
        /// <param name="evt"></param>
        private void OnOwnerSkillCastHandler(SkillCastEvent evt)
        {
            //// 仅响应召唤者的技能释放，且有剩余行动次数
            //if (evt.Caster != Owner || RemainingActionTimes <= 0) return;

            //Console.WriteLine($"\n{Name}响应{Owner.Name}的技能，发动协同攻击！");
            //// 协同攻击（复用角色受伤害API）
            //var summonDamage = Owner.GetAttribute(AttributeType.BaseAtk) * _协同AttackRatio;
            //evt.Target.TakeDamage(summonDamage);
            //Console.WriteLine($"{evt.Target.Name}受到{Name}的协同伤害：{summonDamage}点");

            //// 消耗行动次数
            //ConsumeActionTime();
            //if (RemainingActionTimes <= 0)
            //    Console.WriteLine($"{Name}行动次数耗尽，消失！");
        }

        public bool GetBattleComponent<TComponent>(out TComponent component) where TComponent : IComponent
        {
            bool isTrue = this.TryGetComponent<TComponent>(out TComponent c);
            component = c;
            return isTrue;
        }

        public override void Heal(int value)
        {
            // 召唤物不可回复（可扩展为可回复）
        }

        //public override void TakeDamage(int damage, E_ElementType propertyType)
        //{
        //    // 召唤物不可被攻击（可扩展为可被攻击）
        //}

        protected override IEnumerator OnExceuteAction()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnPreTakeDamage(DamageResult damageResult)
        {
            throw new System.NotImplementedException();
        }
    }
}
