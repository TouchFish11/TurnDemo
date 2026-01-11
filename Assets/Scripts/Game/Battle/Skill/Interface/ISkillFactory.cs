using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能工厂接口
/// </summary>
public interface ISkillFactory
{
    /// <summary>
    /// 创建技能实例
    /// </summary>
    /// <param name="skillId"></param>
    /// <returns></returns>
    ISkill CreateSkill(IBattleEntityObject caster, int skillId);
    
    /// <summary>
    /// 批量创建技能实例
    /// </summary>
    /// <param name="skillIds"></param>
    /// <returns></returns>
    IEnumerable<ISkill> CreateSkills(IBattleEntityObject caster, params int[] skillIds);
}
