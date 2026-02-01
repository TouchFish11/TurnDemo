using Game.Battle.Context;
using Game.Battle.Objects;

namespace GameHotUpdate.Battle.Event.UI
{
    /// <summary>
    /// ��Ҵ����սἼ���¼�
    /// </summary>
    public class PlayerTriggerUltimateSkillEvent : PlayerTriggerSkillEvent
    {
        public PlayerTriggerUltimateSkillEvent(IBattleContext context, IBattleEntityObject battleEntity, int ultimateSkillId) : base(context, ultimateSkillId, battleEntity)
        {

        }
    }
}
