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
        
        public float CurrentValue { get; private set; }
        
        public float MaxValue { get; private set; }

        public ResourceChangedEvent(IBattleContext context, IBattleEntityObject target, float currentEnergy, float maxEnergy) : base(context)
        {
            Target = target;
            CurrentValue = currentEnergy;
            MaxValue = maxEnergy;
        }
    }
}
