using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能工厂
/// </summary>
public abstract class SkillFactory : ISkillFactory
{
    /// <summary>
    /// 批量创建技能对象
    /// </summary>
    /// <param name="skillIds"></param>
    /// <returns></returns>
    public IEnumerable<ISkill> CreateSkills(params int[] skillIds)
    {
        List<ISkill> skills = new List<ISkill>();
        foreach (int skillId in skillIds)
        {
            skills.Add(CreateSkill(skillId));
        }
        return skills;
    }

    /// <summary>
    /// 创建技能对象
    /// </summary>
    /// <param name="skillId"></param>
    /// <returns></returns>
    public abstract ISkill CreateSkill(int skillId);
}
