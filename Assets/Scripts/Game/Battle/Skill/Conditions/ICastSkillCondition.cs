using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能施法条件接口
/// </summary>
public interface ICastSkillCondition
{
    /// <summary>
    /// 能否释放
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    bool CanCast(IBattleEntityObject caster, ISkill skill);
}
