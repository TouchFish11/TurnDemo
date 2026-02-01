using Game.Battle.Context;
using Game.Battle.Event;

namespace GameHotUpdate.Battle.Event.UI
{
    /// <summary>
    /// ����ͷż����¼�
    /// �������սἼ
    /// ���ء��������UI
    /// </summary>
    public class PlayerReleaseSkillEvent : BattleEvent
    {
        public PlayerReleaseSkillEvent(IBattleContext context) : base(context)
        {

        }
    }
}
