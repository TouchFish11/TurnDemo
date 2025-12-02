
namespace Game.Battle
{
    /// <summary>
    /// 破盾事件
    /// </summary>
    public class ToughnessBrokenEvent : BattleEvent
    {
        /// <summary>
        /// 破盾者（技能释放者）
        /// </summary>
        public IBattleEntityObject Breaker { get; }

        /// <summary>
        /// 被破盾的目标
        /// </summary>
        public IBattleEntityObject Target { get; }

        public ToughnessBrokenEvent(IBattleContext context, IBattleEntityObject breaker, IBattleEntityObject target) : base(context)
        {
            Breaker = breaker;
            Target = target;
        }
    }
}
