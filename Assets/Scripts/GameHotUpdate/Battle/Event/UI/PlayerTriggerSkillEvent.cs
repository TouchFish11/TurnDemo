using Game.Battle.Context;
using Game.Battle.Event;
using Game.Battle.Objects;

namespace GameHotUpdate.Battle.Event.UI
{
    /// <summary>
    /// ��Ҵ��������¼�
    /// �������սἼ�Ĵ���
    /// </summary>
    public class PlayerTriggerSkillEvent : BattleEvent
    {
        public int SkillId { get; private set; }

        public IBattleEntityObject Caster { get; private set; }

        public PlayerTriggerSkillEvent(IBattleContext context, int skillId, IBattleEntityObject battleEntity) : base(context)
        {
            SkillId = skillId;
            Caster = battleEntity;
        }
    }
}
