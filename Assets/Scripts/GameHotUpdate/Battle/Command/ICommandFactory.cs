using Core.Reflection;
using GameHotUpdate.Battle.Skill.Interface;
using GameHotUpdate.Battle.Toughness;

namespace GameHotUpdate.Battle.Command
{
    public interface ICommandFactory : IFactory
    {
        ISkillCommand GetSkillCommand(ISkillData skillData);

        IMonsterActCommand GetMonsterActCommand(IToughnessComponent component, ISkillData skillData);
    }
}
