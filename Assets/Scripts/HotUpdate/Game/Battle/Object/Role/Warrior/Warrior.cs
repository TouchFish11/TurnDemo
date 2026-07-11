using Core.DI;
using HotUpdate.Game.Battle.Object.Role.Warrior.Skill;
using HotUpdate.Game.Battle.Skill.Factory;

namespace HotUpdate.Game.Battle.Object.Role.Warrior
{
    /// <summary>
    /// 战士脚本
    /// </summary>
    public class Warrior : PlayerObject
    {
        protected override ISkillFactory GetSkillFactory()
        {
            return DIContainer.Create<WarriorSkillFactory>();
        }
    }
}
