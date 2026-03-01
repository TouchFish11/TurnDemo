using GameHotUpdate.Battle.Context;
using GameHotUpdate.Battle.Object.Role.Wizard.Skill;
using GameHotUpdate.Battle.Skill.Component;

namespace GameHotUpdate.Battle.Object.Role.Wizard
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