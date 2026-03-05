using HotUpdate.Battle.Skill.Base;
using HotUpdate.Battle.Skill.Handler;
using HotUpdate.Data;

namespace HotUpdate.Battle.Skill.Interface
{
    public interface ISkillData : IData
    {
        ISkill Skill { get; }
        ISkillCastPostHandler SkillCastPostHandler { get; }
    }
}
