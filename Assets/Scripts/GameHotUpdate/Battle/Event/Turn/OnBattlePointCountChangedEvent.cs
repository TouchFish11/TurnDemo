using Game.Battle.Context;
using Game.Battle.Event;

namespace GameHotUpdate.Battle.Event.Turn
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
