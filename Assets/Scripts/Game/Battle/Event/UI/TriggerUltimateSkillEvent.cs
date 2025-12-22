using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 触发终结技能事件
    /// </summary>
    public class TriggerUltimateSkillEvent : BattleEvent
    {
        public int UltimateSkillId { get; set; }

        public TriggerUltimateSkillEvent(IBattleContext context) : base(context)
        {

        }
    }
}
