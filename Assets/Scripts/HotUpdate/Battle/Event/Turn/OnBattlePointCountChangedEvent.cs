using HotUpdate.Battle.Context;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Event;

namespace HotUpdate.Battle.Event.Turn
{
    public class OnBattlePointCountChangedEvent : BattleEvent
    {
        public int CurentBattlePointCount { get; private set; }

        public int MaxBattlePointCount { get; private set; }

        public OnBattlePointCountChangedEvent(IBattleContext context, int curentBattlePointCount, int maxBattlePointCount) : base(context)
        {
            CurentBattlePointCount = curentBattlePointCount;
            MaxBattlePointCount = maxBattlePointCount;
        }
    }
}
