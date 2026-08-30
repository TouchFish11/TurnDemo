using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.StatSystem;

namespace HotUpdate.Game.Battle.Event.General
{
    /// <summary>
    /// 属性变化事件
    /// </summary>
    public class StatChangedEvent : BattleEvent
    {
        public IReadOnlyStat Stat { get; }
        
        public StatChangedEvent(IBattleContext context, IReadOnlyStat stat) : base(context)
        {
            Stat = stat;
        }
    }
}
