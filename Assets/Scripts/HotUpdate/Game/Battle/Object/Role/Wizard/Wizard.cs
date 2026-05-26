using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object.Role.Wizard.Skill;
using HotUpdate.Game.Battle.Skill.Component;

namespace HotUpdate.Game.Battle.Object.Role.Wizard
{
    /// <summary>
    /// Herta��ɫ��
    /// </summary>
    public class Wizard : PlayerObject
    {
        public override void BattleInit(int battleEntityId, IBattleContext context)
        {
            base.BattleInit(battleEntityId, context);
            GetComponent<SkillComponent>().InitSkills(RoleInfo.f_skillIds, new WizardSkillFactory());
        }
    }
}