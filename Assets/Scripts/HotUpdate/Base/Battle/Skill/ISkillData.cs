using HotUpdate.Base.Data;

namespace HotUpdate.Base.Battle.Skill
{
    public interface ISkillData
    {
        ISkill Skill { get; }
        
        ISkillCastPostHandler SkillCastPostHandler { get; }
    }
}
