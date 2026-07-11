using System;
using System.Collections.Generic;
using Core.DI;
using Core.Serialize.Binary;

using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Component;

namespace HotUpdate.Game.Battle.Operation.Provider
{
    public class UltimateSkillKeyUIDataProvider : ISkillKeyUIDataProvider
    {
        [Inject] private IBinaryDataManager _binaryDataManager;
        
        public SkillKeyUIData GetData(IBattleEntityObject provider)
        {
            var skillKeyUIData = new SkillKeyUIData(new List<SkillInfo>(), provider);

            ISkill skill = null;
            var skillComponent = provider.GetComponent<ISkillComponent>();
            foreach (var skillId in skillComponent.GetSkillIds())
            {
                var skillInfo = _binaryDataManager.GetConfig<SkillInfoContainer>(EConfigLoadType.Excel).dataDic[skillId];
                if (skillInfo.f_SkillType == (int)E_SkillType.UltimateSkill)
                {
                    skill = skillComponent.GetSkill(skillId);
                }
            }
            
            if(skill == null)
                throw new NullReferenceException(nameof(skill));
            
            skillKeyUIData.SkillInfos.Add(skill.SkillContext.SkillInfo);
            return skillKeyUIData;
        }
    }
}
