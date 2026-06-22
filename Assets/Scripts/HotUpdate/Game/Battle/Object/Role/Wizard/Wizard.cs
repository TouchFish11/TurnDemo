using Core.DI;
using HotUpdate.Game.Battle.Object.Role.Wizard.Skill;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;

namespace HotUpdate.Game.Battle.Object.Role.Wizard
{
    /// <summary>
    /// 法师对象
    /// </summary>
    public class Wizard : PlayerObject
    {
        protected override void OnBattleInit()
        {
            var skillComponent = GetComponent<ISkillComponent>();
            var core = DIContainer.Create<SkillComponentCore>();
            core.Init(skillComponent, RoleInfo.f_skillIds, DIContainer.Create<WizardSkillFactory>());
            skillComponent.InitSkill(this, core);
        }
    }
}