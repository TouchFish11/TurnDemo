using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 玩家触发终结技能事件
    /// </summary>
    public class PlayerTriggerUltimateSkillEvent : PlayerTriggerSkillEvent
    {
        public PlayerTriggerUltimateSkillEvent(IBattleContext context, IBattleEntityObject battleEntity, int ultimateSkillId) : base(context, ultimateSkillId, battleEntity)
        {

        }
    }
}
