using HotUpdate.Battle.Skill.Handler;
using HotUpdate.Battle.Skill.Interface;

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
        }
    }
}
