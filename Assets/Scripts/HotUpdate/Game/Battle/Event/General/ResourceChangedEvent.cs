using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Event.General
{
    /// <summary>
    /// 能量变化事件
    /// </summary>
    public class ResourceChangedEvent : BattleEvent
    {
        public IBattleEntityObject Target { get; private set; }
        
        public int CurrentValue { get; private set; }
        
        public int MaxValue { get; private set; }

        /// <summary>
        /// 能量变化量
        /// 正数为增加，负数为减少
        /// </summary>
        public int DeltaEnergy { get; private set; }

        public ResourceChangedEvent(IBattleContext context, IBattleEntityObject target, int currentEnergy, int maxEnergy, int deltaEnergy) : base(context)
        {
            Target = target;
            CurrentValue = currentEnergy;
            MaxValue = maxEnergy;
            DeltaEnergy = deltaEnergy;
        }
    }
}
