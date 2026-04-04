using HotUpdate.Core.Data;

namespace HotUpdate.Core.Battle.Skill
{
    public interface ISkillData : IData<ISkillData>
    {
        ISkill Skill { get; }
        
        ISkillCastPostHandler SkillCastPostHandler { get; }
    }
}
