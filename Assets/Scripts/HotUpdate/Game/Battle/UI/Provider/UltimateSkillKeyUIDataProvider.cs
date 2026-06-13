using System.Collections.Generic;
using HotUpdate.Base;
using HotUpdate.Common.Config.ExcelInfo.Info;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Component;

namespace HotUpdate.Game.Battle.UI.Provider
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
