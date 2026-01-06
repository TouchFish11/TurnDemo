using System.Collections.Generic;

namespace Game.Battle
{
    /// <summary>
    /// 技能释放事件
    /// 目前用于播放动画
    /// </summary>
    public class SkillCastEvent : BattleEvent
    {
        /// <summary>
        /// 释放的技能
        /// </summary>
        public ISkill Skill { get; }

        /// <summary>
        /// 技能造成的伤害
        /// </summary>
        public float Damage { get; }

        public SkillCastEvent(IBattleContext context, ISkill skill, float damage) : base(context)
        {
            Skill = skill;
            Damage = damage;
        }

        public bool Contain(IBattleEntityObject battleEntity)
        {
            return Skill.AllTargets.Contains(battleEntity);
        }
    }
}
