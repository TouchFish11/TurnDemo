using Game.Battle.Context;
using Game.Battle.Event;

namespace GameHotUpdate.Battle.Event.UI
{
    /// <summary>
    /// 清除累计伤害UI显示
    /// </summary>
    public class ClearCumulativeDamageEvent : BattleEvent
    {
        public ClearCumulativeDamageEvent(IBattleContext context) : base(context)
        {
            
        }
    }
}
