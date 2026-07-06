using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Damage;

namespace HotUpdate.Game.Battle.Event.General
{
    /// <summary>
    /// 应用伤害事件
    /// </summary>
    public class ApplyDamageEvent : BattleEvent
    {
        /// <summary>
        /// 伤害结果
        /// </summary>
        public DamageResult DamageResult {  get; private set; } 

        public ApplyDamageEvent(IBattleContext context, DamageResult damageResult) : base(context)
        {
            DamageResult = damageResult;
        }
    }
}
