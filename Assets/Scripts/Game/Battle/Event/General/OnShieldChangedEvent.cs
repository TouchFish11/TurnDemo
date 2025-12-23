using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    public class OnShieldChangedEvent : BattleEvent
    {
        public int CurrentShield { get; private set; }
        public int MaxShield { get; private set; }


        public OnShieldChangedEvent(IBattleContext context, int currentShield, int maxShield) : base(context)
        {
            CurrentShield = currentShield;
            MaxShield = maxShield;
        }
    }
}
