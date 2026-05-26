using HotUpdate.Base;
using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Event.General
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
        /// 能量变化量
        /// 正数为增加，负数为减少
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
