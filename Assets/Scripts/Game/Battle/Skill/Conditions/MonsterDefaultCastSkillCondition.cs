using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物默认释放技能条件
/// </summary>
[CastSkillCondition]
public class MonsterDefaultCastSkillCondition : ICastSkillCondition
{
    public bool CanCast(IBattleEntityObject caster, ISkill skill)
    {
        switch (skill.SkillInfo.f_SkillType.ToSkillType())
        {
            case E_SkillType.Monster:
                return true;
            default:
                return false;
        }
    }
}
