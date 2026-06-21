using HotUpdate.Game.Battle.Object.Role.Wizard.Skill;
using HotUpdate.Game.Battle.Skill.Component;

namespace HotUpdate.Game.Battle.Object.Role.Wizard
{
    /// <summary>
    /// 法师对象
    /// </summary>
    public class Wizard : PlayerObject
    {
        protected override void OnBattleInit()
        {
            GetComponent<SkillComponent>().InitSkills(RoleInfo.f_skillIds, new WizardSkillFactory());
        }
    }
}