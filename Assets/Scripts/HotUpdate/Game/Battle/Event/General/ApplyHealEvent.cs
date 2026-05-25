using HotUpdate.Base;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Event.General
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
