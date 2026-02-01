using Game.Battle.Context;
using Game.Battle.Event;
using Game.Battle.Objects;
using Game.Battle.Skill;

namespace GameHotUpdate.Battle.Event
{
    /// <summary>
    /// �����ͷ��¼�
    /// Ŀǰ���ڲ��Ŷ���
    /// </summary>
    public class SkillCastEvent : BattleEvent
    {
        /// <summary>
        /// �ͷŵļ���
        /// </summary>
        public ISkill Skill { get; }

        /// <summary>
        /// ������ɵ��˺�
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
