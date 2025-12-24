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
        public int SkillId { get; private set; }

        public SelectSkillEvent(IBattleContext context, int skillId) : base(context)
        {
            SkillId = skillId;
        }
    }
}
