using Core.Reflection;
using HotUpdate.Battle.Skill.Interface;
using HotUpdate.Battle.Toughness;

namespace HotUpdate.Battle.Command
{
    public interface ICommandFactory : IFactory
    {
        ISkillCommand GetSkillCommand(ISkillData skillData);

        IMonsterActCommand GetMonsterActCommand(IToughnessComponent component, ISkillData skillData);
    }
}
