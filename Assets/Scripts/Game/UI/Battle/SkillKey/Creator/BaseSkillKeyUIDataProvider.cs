using Game.Battle;
using System.Collections.Generic;

/// <summary>
/// 基础技能按键UI数据提供器
/// </summary>
public class BaseSkillKeyUIDataProvider : ISkillKeyUIDataProvider
{
    public SkillKeyUIData GetData(IBattleEntityObject provider)
    {
        SkillKeyUIData skillKeyUIData = new SkillKeyUIData(new List<SkillInfo>(), provider);
        List<ISkill> skills = new List<ISkill>(provider.GetComponent<SkillComponent>().GetSkills());

        // 遍历技能
        foreach (ISkill skill in skills)
        {
            SkillInfo skillInfo = skill.SkillInfo;
            if ((E_SkillType)skillInfo.f_SkillType == E_SkillType.UltimateSkill)
            {
                continue;
            }

            skillKeyUIData.SkillInfos.Add(skillInfo);
        }

        return skillKeyUIData;
    }
}
