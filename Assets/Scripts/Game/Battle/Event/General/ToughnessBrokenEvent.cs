
namespace Game.Battle
{
    /// <summary>
    /// 破韧事件
    /// </summary>
    public class ToughnessBrokenEvent : BattleEvent
    {
        /// <summary>
        /// 破韧者
        /// </summary>
        public IBattleEntityObject Breaker { get; }

        /// <summary>
        /// 被破韧的目标
        /// </summary>
        public IBattleEntityObject Target { get; }

        /// <summary>
        /// 造成击破的技能信息
        /// </summary>
        public SkillInfo SkillInfo { get; }

        public ToughnessBrokenEvent(IBattleContext context, IBattleEntityObject breaker, IBattleEntityObject target, SkillInfo skillInfo) : base(context)
        {
            Breaker = breaker;
            Target = target;
            SkillInfo = skillInfo;
        }
    }
}
