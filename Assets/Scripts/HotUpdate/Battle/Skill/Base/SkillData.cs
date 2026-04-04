using System;
using HotUpdate.Core.Battle.Skill;

namespace HotUpdate.Battle.Skill.Base
{
    public struct SkillData : ISkillData
    {
        public ISkill Skill { get; }
        
        public ISkillCastPostHandler SkillCastPostHandler { get; }

        public SkillData(ISkill skill, ISkillCastPostHandler skillCastPostHandler)
        {
            Skill = skill;
            SkillCastPostHandler = skillCastPostHandler;
            OnDataChanged = null;
        }

        public event Action<ISkillData> OnDataChanged;
    }
}
