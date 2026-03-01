using GameHotUpdate.Battle.Skill.Base;
using GameHotUpdate.Battle.Skill.Handler;
using GameHotUpdate.Data;

namespace GameHotUpdate.Battle.Skill.Interface
{
    public interface ISkillData : IData
    {
        ISkill Skill { get; }
        ISkillCastPostHandler SkillCastPostHandler { get; }
    }
}
