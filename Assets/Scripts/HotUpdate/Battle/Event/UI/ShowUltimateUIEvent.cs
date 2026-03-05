using HotUpdate.Battle.Context;
using HotUpdate.Battle.Object;
using HotUpdate.Battle.Skill.Base;

namespace HotUpdate.Battle.Event.UI
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
