using HotUpdate.Battle.Context;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Event;

namespace HotUpdate.Battle.Event.Skill
{
    /// <summary>
    /// 终结技释放事件
    /// </summary>
    public class UltimateCastEvent : BattleEvent
    {
        public UltimateCastEvent(IBattleContext context) : base(context)
        {
        
        }
    }
}
