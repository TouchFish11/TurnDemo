using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 玩家触发技能事件
    /// 不处理终结技的触发
    /// </summary>
    public class PlayerTriggerSkillEvent : BattleEvent
    {
        public int SkillId { get; private set; }

        public IBattleEntityObject Caster { get; private set; }

        public PlayerTriggerSkillEvent(IBattleContext context, int skillId, IBattleEntityObject battleEntity) : base(context)
        {
            SkillId = skillId;
            Caster = battleEntity;
        }
    }
}
