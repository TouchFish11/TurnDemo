using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Event;

namespace HotUpdate.Game.Battle.Event.Turn
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
