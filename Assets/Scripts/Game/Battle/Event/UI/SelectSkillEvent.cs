using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 选择技能事件
    /// 非终结技使用
    /// </summary>
    public class SelectSkillEvent : BattleEvent
    {
        /// <summary>
        /// 技能ID
        /// </summary>
        public int SkillId { get; private set; }

        /// <summary>
        /// 施法者
        /// </summary>
        public IBattleEntityObject Caster { get; private set; }

        public ITargetSelectStrategy TargetSelectStrategy { get; }

        public SelectSkillEvent(IBattleContext context, int skillId, IBattleEntityObject caster, ITargetSelectStrategy targetSelectStrategy) : base(context)
        {
            SkillId = skillId;
            Caster = caster;
            TargetSelectStrategy = targetSelectStrategy;
        }
    }
}
