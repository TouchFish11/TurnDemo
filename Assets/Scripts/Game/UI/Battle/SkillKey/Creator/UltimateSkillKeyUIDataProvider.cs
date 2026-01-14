using Game.Battle;
using System.Collections.Generic;

/// <summary>
/// 终结技技能按键UI数据提供器
/// </summary>
public class UltimateSkillKeyUIDataProvider : ISkillKeyUIDataProvider
{
    public SkillKeyUIData GetData(IBattleEntityObject provider)
    {
        SkillKeyUIData skillKeyUIData = new SkillKeyUIData(new List<SkillInfo>(), provider);
        List<ISkill> skills = new List<ISkill>(provider.GetComponent<SkillComponent>().GetSkills());

        // 找到终结技技能
        ISkill skill = skills.Find((skill) => (E_SkillType)skill.SkillInfo.f_SkillType == E_SkillType.UltimateSkill);
        skillKeyUIData.SkillInfos.Add(skill.SkillInfo);
        return skillKeyUIData;
    }
}
