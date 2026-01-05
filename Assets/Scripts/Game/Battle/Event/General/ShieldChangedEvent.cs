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
        public IBattleEntityObject Target { get; }

        public int CurrentShield { get; }

        /// <summary>
        /// 护盾变化值
        /// 原始 - 新值。正数为减少，负数为增加
        /// </summary>
        public int DeltaShield { get; }

        /// <summary>
        /// 护盾基准值
        /// </summary>
        public int ReferenceShield { get; } = 10000;


        public ShieldChangedEvent(IBattleContext context, int currentShield, IBattleEntityObject target, int deltaShield) : base(context)
        {
            CurrentShield = currentShield;
            Target = target;
            DeltaShield = deltaShield;
        }
    }
}
