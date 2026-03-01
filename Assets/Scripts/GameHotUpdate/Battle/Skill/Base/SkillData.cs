using GameHotUpdate.Battle.Skill.Handler;
using GameHotUpdate.Battle.Skill.Interface;

namespace GameHotUpdate.Battle.Skill.Base
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
