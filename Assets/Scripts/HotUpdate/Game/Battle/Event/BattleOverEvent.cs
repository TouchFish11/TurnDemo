using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Event
{
    /// <summary>
    /// 战斗结束事件
    /// </summary>
    public class BattleOverEvent : BattleEvent
    {
        public BattleOverEvent(IBattleContext context) : base(context)
        {

        }
    }
}
