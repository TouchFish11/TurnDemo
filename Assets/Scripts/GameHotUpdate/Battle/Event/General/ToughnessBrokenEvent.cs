using GameHotUpdate.Battle.Context;
using GameHotUpdate.Battle.Object;

namespace GameHotUpdate.Battle.Event.General
{
    /// <summary>
    /// 击破事件
    /// </summary>
    public class ToughnessBrokenEvent : BattleEvent
    {
        /// <summary>
        /// 
        /// </summary>
        public IBattleEntityObject Breaker { get; }

        /// <summary>
        /// 
        /// </summary>
        public IBattleEntityObject Target { get; }
        
        /// <summary>
        /// 削韧量
        /// 没有则为0
        /// </summary>
        public int ResilienceValue { get; }
        
        /// <summary>
        /// 技能ID
        /// </summary>
        public int SkillId { get; }

        public ToughnessBrokenEvent(IBattleContext context, IBattleEntityObject breaker, IBattleEntityObject target, int resilienceValue, int skillId) : base(context)
        {
            Breaker = breaker;
            Target = target;
            ResilienceValue = resilienceValue;
            SkillId = skillId;
        }
    }
}
