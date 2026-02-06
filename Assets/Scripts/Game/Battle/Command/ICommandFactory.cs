using Core.Reflection;
using Game.Battle.Skill.Interface;
using Game.Battle.Toughness;

namespace Game.Battle.Command
{
    public interface ICommandFactory : IFactory
    {
        ISkillCommand GetSkillCommand(ISkillData skillData);

        IMonsterActCommand GetMonsterActCommand(IToughnessComponent component, ISkillData skillData);
    }
}
