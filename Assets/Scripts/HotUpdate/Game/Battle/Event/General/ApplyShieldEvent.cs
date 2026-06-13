using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Event.General
{
    /// <summary>
    /// 应用护盾事件
    /// </summary>
    public class ApplyShieldEvent : BattleEvent
    {
        public IBattleEntityObject Target { get; }
        
        /// <summary>
        /// 护盾量
        /// </summary>
        public int ShieldAmount { get; }
        
        public ApplyShieldEvent(IBattleContext context, IBattleEntityObject target, int shieldAmount) : base(context)
        {
            Target = target;
            ShieldAmount = shieldAmount;
        }
    }
}
