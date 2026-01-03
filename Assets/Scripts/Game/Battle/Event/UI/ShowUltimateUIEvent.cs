using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 显示终结UI事件
    /// </summary>
    public class ShowUltimateUIEvent : BattleEvent
    {
        public ISkill Skill { get; private set; }

        public IBattleEntityObject Caster { get; private set; }

        public ShowUltimateUIEvent(IBattleContext context, ISkill skill, IBattleEntityObject caster) : base(context)
        {
            Skill = skill;
            Caster = caster;
        }
    }
}
