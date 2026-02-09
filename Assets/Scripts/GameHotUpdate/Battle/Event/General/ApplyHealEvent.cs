using Game.Battle.Context;
using Game.Battle.Event;
using Game.Battle.Objects;

namespace GameHotUpdate.Battle.Event.General
{
    /// <summary>
    /// 应用治疗事件
    /// </summary>
    public class ApplyHealEvent : BattleEvent
    {
        public IBattleEntityObject Target { get; }
        
        /// <summary>
        /// 治疗量
        /// </summary>
        public int HealAmount { get; }
        
        public ApplyHealEvent(IBattleContext context, IBattleEntityObject target, int healAmount) : base(context)
        {
            Target = target;
            HealAmount = healAmount;
        }
    }
}
