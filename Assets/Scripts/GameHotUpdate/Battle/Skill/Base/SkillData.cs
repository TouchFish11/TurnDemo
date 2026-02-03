using Game.Battle.Skill;
using Game.Battle.Skill.Handler;
using Game.Battle.Skill.Interface;

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
