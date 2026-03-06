using HotUpdate.Battle.Context;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Event;

namespace HotUpdate.Battle.Event.UI
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
