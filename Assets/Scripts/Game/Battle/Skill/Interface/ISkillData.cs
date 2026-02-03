using Game.Battle.Skill.Handler;
using Game.Data;

namespace Game.Battle.Skill.Interface
{
    public interface ISkillData : IData
    {
        ISkill Skill { get; }
        ISkillCastPostHandler SkillCastPostHandler { get; }
    }
}
