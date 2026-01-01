using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 韧性变化事件
    /// </summary>
    public class ToughnessChangedEvent : BattleEvent
    {
        public IBattleEntityObject Target { get; private set; }
        public int CurrentToughness { get; private set; }
        public int MaxToughness { get; private set; }

        public ToughnessChangedEvent(IBattleContext context, IBattleEntityObject battleEntity, int currentToughness, int maxToughness) : base(context)
        {
            this.Target = battleEntity;
            this.CurrentToughness = currentToughness;
            this.MaxToughness = maxToughness;
        }
    }
}
