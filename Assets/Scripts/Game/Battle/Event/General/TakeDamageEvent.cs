using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// ÊÜÉËÊÂ¼ş
    /// </summary>
    public class TakeDamageEvent : BattleEvent
    {
        public DamageResult DamageResult {  get; private set; } 

        public TakeDamageEvent(IBattleContext context, DamageResult damageResult) : base(context)
        {
            DamageResult = damageResult;
        }
    }
}
