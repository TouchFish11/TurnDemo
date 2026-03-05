using System.Collections.Generic;
using HotUpdate.Battle.Context;
using HotUpdate.Battle.Skill.Base;

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
