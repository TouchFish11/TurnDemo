using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 弱点变化事件
    /// </summary>
    public class WeaknessChangeEvent : BattleEvent
    {
        public WeaknessChangeEvent(IBattleContext context) : base(context)
        {

        }
    }
}
