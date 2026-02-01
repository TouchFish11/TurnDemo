using Game.Battle.Context;
using Game.Battle.Event;
using Game.Battle.Objects;
using Game.Battle.Skill;

namespace GameHotUpdate.Battle.Event.UI
{
    /// <summary>
    /// ��ʾ�սἼ����UI�¼�
    /// </summary>
    public class ShowUltimateUIEvent : BattleEvent
    {
        public ISkill Skill { get; private set; }

        public IBattleEntityObject Caster { get; private set; }

        public ShowUltimateUIEvent(IBattleContext context, ISkill skill, IBattleEntityObject caster) : base(context)
        {
            Skill = skill;
            Caster = caster;
        }
    }
}
