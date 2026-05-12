using System;
using HotUpdate.Base.Battle.Skill;

namespace HotUpdate.Game.Battle.Skill.Base
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
