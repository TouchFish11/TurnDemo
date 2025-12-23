using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
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
