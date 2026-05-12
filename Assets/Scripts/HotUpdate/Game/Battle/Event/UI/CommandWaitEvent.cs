using System.Collections.Generic;
using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Event;
using HotUpdate.Base.Battle.Skill;

namespace HotUpdate.Game.Battle.Event.UI
{
    /// <summary>
    /// ָ��ȴ��¼�
    /// </summary>
    public class CommandWaitEvent : BattleEvent
    {
        public List<ISkill> WaitingSkills { get; }

        public CommandWaitEvent(IBattleContext context, List<ISkill> waitingSkills) : base(context)
        {
            WaitingSkills = waitingSkills;
        }
    }
}
