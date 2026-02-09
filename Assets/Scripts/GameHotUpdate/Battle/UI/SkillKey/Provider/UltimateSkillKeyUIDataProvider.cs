using System.Collections.Generic;
using Game.Battle.Objects;
using Game.Battle.Skill;
using Game.Battle.Skill.Component;
using Game.Battle.Skill.Enum;
using Game.UI.Battle.SkillKey.Provider;

namespace GameHotUpdate.Battle.UI.SkillKey.Provider
{
    /// <summary>
    /// �սἼ���ܰ���UI�����ṩ��
    /// </summary>
    public class UltimateSkillKeyUIDataProvider : ISkillKeyUIDataProvider
    {
        public SkillKeyUIData GetData(IBattleEntityObject provider)
        {
            SkillKeyUIData skillKeyUIData = new SkillKeyUIData(new List<SkillInfo>(), provider);
            List<ISkill> skills = new List<ISkill>(provider.GetComponent<SkillComponent>().GetSkills());

            // �ҵ��սἼ����
            ISkill skill = skills.Find((skill) => (E_SkillType)skill.SkillInfo.f_SkillType == E_SkillType.UltimateSkill);
            skillKeyUIData.SkillInfos.Add(skill.SkillInfo);
            return skillKeyUIData;
        }
    }
}
