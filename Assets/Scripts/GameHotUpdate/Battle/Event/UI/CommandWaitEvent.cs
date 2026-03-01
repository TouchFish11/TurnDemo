using System.Collections.Generic;
using GameHotUpdate.Battle.Context;
using GameHotUpdate.Battle.Skill.Base;

namespace GameHotUpdate.Battle.Event.UI
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
