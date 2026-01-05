
using System.Collections.Generic;

namespace Game.Battle
{
    /// <summary>
    /// 指令等待事件
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
