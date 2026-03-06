using HotUpdate.Core.Data;

namespace HotUpdate.Core.Battle.Skill
{
    public interface ISkillData : IData
    {
        ISkill Skill { get; }
        
        ISkillCastPostHandler SkillCastPostHandler { get; }
    }
}
