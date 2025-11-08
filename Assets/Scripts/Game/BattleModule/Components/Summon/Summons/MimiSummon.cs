using GameLogic.BattleMoudule.Entity;
using GameLogic.BattleMoudule.Event;
using GameLogic.BattleMoudule.Relic;
using System.Collections;
using UnityEngine;

namespace GameLogic.BattleMoudule.Summon
{
    /// <summary>
    /// 迷迷召唤物
    /// </summary>
    public class MimiSummon : MonoBehaviour, ISummon
    {
        public string Name => "迷迷";

        public IBattleEntity Owner { get; private set; }

        public MimiSummon()
        {

        }

        public void Init(IBattleEntity owner)
        {
            Owner = owner;
            // 订阅“主人技能释放事件”（主人放技能时，召唤物协同攻击）(可选)
            //BattleEventCenter.AddListener<SkillCastEvent>(OnOwnerSkillCastHandler);
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
            //var summonDamage = Owner.GetAttribute(AttributeType.Attack) * _协同AttackRatio;
            //evt.Target.TakeDamage(summonDamage);
            //Console.WriteLine($"{evt.Target.Name}受到{Name}的协同伤害：{summonDamage}点");

            //// 消耗行动次数
            //ConsumeActionTime();
            //if (RemainingActionTimes <= 0)
            //    Console.WriteLine($"{Name}行动次数耗尽，消失！");
        }

        public IEnumerator ExecuteAction()
        {
            throw new System.NotImplementedException();
        }

        public bool GetBattleComponent<TComponent>(out TComponent component) where TComponent : IComponent
        {
            bool isTrue = this.TryGetComponent<TComponent>(out TComponent c);
            component = c;
            return isTrue;
        }

        public int GetField(E_FieldType propertyType)
        {
            // 属性依赖主人(可选)
            return (int)(Owner.GetField(propertyType) * 0.8f);
        }

        public int GetSpeed()
        {
            // 召唤物不参与行动队列，仅协同（可扩展为参与行动队列）
            return 0;
        }

        public void Heal(int value)
        {
            // 召唤物不可回复（可扩展为可回复）
        }

        public void TakeDamage(int damage, E_PropertyType propertyType)
        {
            // 召唤物不可被攻击（可扩展为可被攻击）
        }

        public void AddRelicBonus(E_RelicBoun relicBoun, float value)
        {

        }
    }
}
