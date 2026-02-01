using System.Collections.Generic;
using Game.Battle.Context;
using Game.Battle.Event;
using Game.Battle.Skill;

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
