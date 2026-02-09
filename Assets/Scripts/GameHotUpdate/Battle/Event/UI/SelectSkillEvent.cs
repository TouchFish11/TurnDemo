using Game.Battle.Context;
using Game.Battle.Event;
using Game.Battle.Objects;
using Game.Battle.TargetSelect;

namespace GameHotUpdate.Battle.Event.UI
{
    /// <summary>
    /// 选择技能事件
    /// 角色使用
    /// </summary>
    public class SelectSkillEvent : BattleEvent
    {
        /// <summary>
        /// 技能ID
        /// </summary>
        public int SkillId { get; private set; }

        /// <summary>
        /// 释放者
        /// </summary>
        public IBattleEntityObject Caster { get; private set; }

        /// <summary>
        /// 目标选择策略
        /// </summary>
        public ITargetSelectStrategy TargetSelectStrategy { get; }

        public SelectSkillEvent(IBattleContext context, int skillId, IBattleEntityObject caster, ITargetSelectStrategy targetSelectStrategy) : base(context)
        {
            SkillId = skillId;
            Caster = caster;
            TargetSelectStrategy = targetSelectStrategy;
        }
    }
}
