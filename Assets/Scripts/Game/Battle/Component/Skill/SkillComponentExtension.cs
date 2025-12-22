using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillComponentExtension
{
    /// <summary>
    /// 获取终结技ID
    /// </summary>
    /// <param name="skillComponent"></param>
    /// <returns>若未找到ID，则返回-1</returns>
    public static int GetUltimateSkill(this SkillComponent skillComponent)
    {
        List<ISkill> skills = new List<ISkill>(skillComponent.GetSkills());

        foreach (ISkill skill in skills)
        {
            if (skill.SkillInfo.f_SkillType.ToSkillType() == E_SkillType.UltimateSkill)
            {
                return skill.SkillInfo.f_id;
            }
        }

        LogManager.Log($"[SkillComponentExtension] 未找到终结技ID，返回-1");
        return -1;
    }
}
