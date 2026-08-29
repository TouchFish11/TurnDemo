using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Property.New.Test.StatSystem;

namespace HotUpdate.Game.Battle.Event.General
{
    /// <summary>
    /// 属性变化事件
    /// </summary>
    public class StatChangedEvent : BattleEvent
    {
        public Stat Stat { get; }
        
        public StatChangedEvent(IBattleContext context, Stat stat) : base(context)
        {
            Stat = stat;
        }
    }
}
