using Core.Reflection;
using HotUpdate.Base.Battle.Skill;
using HotUpdate.Base.Battle.Toughness;

namespace HotUpdate.Base.Battle.Command
{
    public interface ICommandFactory : IFactory
    {
        ISkillCommand GetSkillCommand(ISkillData skillData);

        IMonsterActCommand GetMonsterActCommand(IToughnessComponent component, ISkillData skillData);
    }
}
