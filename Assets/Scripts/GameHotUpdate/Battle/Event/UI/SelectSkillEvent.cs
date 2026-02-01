using Game.Battle.Context;
using Game.Battle.Event;
using Game.Battle.Objects;
using Game.Battle.TargetSelect;

namespace GameHotUpdate.Battle.Event.UI
{
    /// <summary>
    /// ѡ�����¼�
    /// ���սἼʹ��
    /// </summary>
    public class SelectSkillEvent : BattleEvent
    {
        /// <summary>
        /// ����ID
        /// </summary>
        public int SkillId { get; private set; }

        /// <summary>
        /// ʩ����
        /// </summary>
        public IBattleEntityObject Caster { get; private set; }

        public ITargetSelectStrategy TargetSelectStrategy { get; }

        public SelectSkillEvent(IBattleContext context, int skillId, IBattleEntityObject caster, ITargetSelectStrategy targetSelectStrategy) : base(context)
        {
            SkillId = skillId;
            Caster = caster;
            TargetSelectStrategy = targetSelectStrategy;
        }
    }
}
