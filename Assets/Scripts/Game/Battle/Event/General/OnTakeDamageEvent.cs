using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Battle
{
    public class OnTakeDamageEvent : BattleEvent
    {
        public DamageResult DamageResult {  get; private set; } 

        public OnTakeDamageEvent(IBattleContext context, DamageResult damageResult) : base(context)
        {
            DamageResult = damageResult;
        }
    }
}
