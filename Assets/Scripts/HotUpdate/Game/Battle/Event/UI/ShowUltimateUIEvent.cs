using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Event;
using HotUpdate.Base.Battle.Object;
using HotUpdate.Base.Battle.Skill;

namespace HotUpdate.Game.Battle.Event.UI
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
