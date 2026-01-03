using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 能量变化事件
    /// </summary>
    public class EnergyChangedEvent : BattleEvent
    {
        public IBattleEntityObject Target { get; private set; }
        public int CurrentEnergy { get; private set; }
        public int MaxEnergy { get; private set; }

        /// <summary>
        /// 能量差值（原数值 - 现数值）
        /// 正数为能量减少负数为能量增加
        /// </summary>
        public int DeltaEnergy { get; private set; }

        public EnergyChangedEvent(IBattleContext context, IBattleEntityObject target, int currentEnergy, int maxEnergy, int deltaEnergy) : base(context)
        {
            Target = target;
            CurrentEnergy = currentEnergy;
            MaxEnergy = maxEnergy;
            DeltaEnergy = deltaEnergy;
        }
    }
}
