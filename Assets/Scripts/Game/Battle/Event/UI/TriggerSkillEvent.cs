using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    public class TriggerSkillEvent : BattleEvent
    {
        public int SkillId { get; private set; }

        public IBattleEntityObject BattleEntity { get; private set; }

        public TriggerSkillEvent(IBattleContext context, int skillId, IBattleEntityObject battleEntity) : base(context)
        {
            SkillId = skillId;
            BattleEntity = battleEntity;
        }
    }
}
