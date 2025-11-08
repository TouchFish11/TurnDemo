using GameLogic.BattleMoudule.Core;

namespace GameLogic.BattleMoudule.Event
{
    /// <summary>
    /// 破盾事件
    /// </summary>
    public class ToughnessBrokenEvent : BattleEvent
    {
        /// <summary>
        /// 破盾者（技能释放者）
        /// </summary>
        public IBattleEntity Breaker { get; }

        /// <summary>
        /// 被破盾的目标
        /// </summary>
        public IBattleEntity Target { get; }

        public ToughnessBrokenEvent(IBattleContext context, IBattleEntity breaker, IBattleEntity target) : base(context)
        {
            Breaker = breaker;
            Target = target;
        }
    }
}
