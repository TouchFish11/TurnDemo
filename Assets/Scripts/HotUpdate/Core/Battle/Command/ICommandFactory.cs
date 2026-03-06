using Core.Reflection;
using HotUpdate.Core.Battle.Skill;
using HotUpdate.Core.Battle.Toughness;

namespace HotUpdate.Core.Battle.Command
{
    public interface ICommandFactory : IFactory
    {
        ISkillCommand GetSkillCommand(ISkillData skillData);

        IMonsterActCommand GetMonsterActCommand(IToughnessComponent component, ISkillData skillData);
    }
}
