using Core.DI;
using HotUpdate.Game.Battle.Object.Role.Wizard.Skill;
using HotUpdate.Game.Battle.Skill;

namespace HotUpdate.Game.Battle.Object.Role.Wizard
{
    /// <summary>
    /// 法师对象
    /// </summary>
    public class Wizard : PlayerObject
    {
        protected override ISkillFactory GetSkillFactory()
        {
            return DIContainer.Create<WizardSkillFactory>();
        }
    }
}