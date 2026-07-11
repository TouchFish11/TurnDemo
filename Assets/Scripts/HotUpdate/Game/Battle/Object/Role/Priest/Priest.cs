using Core.DI;
using HotUpdate.Game.Battle.Object.Role.Priest.Skill;
using HotUpdate.Game.Battle.Skill.Factory;

namespace HotUpdate.Game.Battle.Object.Role.Priest
{
    /// <summary>
    /// 牧师脚本
    /// </summary>
    public class Priest : PlayerObject
    {
        protected override ISkillFactory GetSkillFactory()
        {
            return DIContainer.Create<PriestSkillFactory>();
        }
    }
}
