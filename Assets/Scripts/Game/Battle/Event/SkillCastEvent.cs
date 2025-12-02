using GameLogic.BattleMoudule.Entity;
using System.Collections.Generic;

namespace Game.Battle
{
    /// <summary>
    /// 技能释放事件
    /// </summary>
    public class SkillCastEvent : BattleEvent
    {
        /// <summary>
        /// 技能释放者
        /// </summary>
        public IBattleEntityObject Caster { get; }

        /// <summary>
        /// 技能目标
        /// </summary>
        public List<IBattleEntityObject> Targets { get; }

        /// <summary>
        /// 释放的技能
        /// </summary>
        public ISkill Skill { get; }

        /// <summary>
        /// 技能造成的伤害
        /// </summary>
        public float Damage { get; }

        /// <summary>
        /// 技能属性
        /// </summary>
        public E_PropertyType PropertyType { get; }

        public SkillCastEvent(IBattleContext context, IBattleEntityObject caster, List<IBattleEntityObject> targets, ISkill skill, float damage, E_PropertyType attackAttr) : base(context)
        {
            Caster = caster;
            Targets = targets;
            Skill = skill;
            Damage = damage;
            PropertyType = attackAttr;
        }

        public bool Contain(IBattleEntityObject battleEntity)
        {
            return Targets.Contains(battleEntity);
        }
    }
}
