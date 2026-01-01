using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 护盾变化事件
    /// </summary>
    public class ShieldChangedEvent : BattleEvent
    {
        public int CurrentShield { get; private set; }
        public int MaxShield { get; private set; }


        public ShieldChangedEvent(IBattleContext context, int currentShield, int maxShield) : base(context)
        {
            CurrentShield = currentShield;
            MaxShield = maxShield;
        }
    }
}
