using System.Collections.Generic;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Event;
using HotUpdate.Core.Battle.Skill;

namespace HotUpdate.Battle.Event.UI
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
