using System.Collections.Generic;
using HotUpdate.Battle.Object;
using HotUpdate.Battle.Skill.Base;
using HotUpdate.Battle.Skill.Component;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.Battle.Skill;

namespace HotUpdate.Battle.UI.SkillKey.Provider
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
