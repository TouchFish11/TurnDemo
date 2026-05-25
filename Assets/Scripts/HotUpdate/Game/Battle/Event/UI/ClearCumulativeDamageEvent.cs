using HotUpdate.Base;
using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Event.UI
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
