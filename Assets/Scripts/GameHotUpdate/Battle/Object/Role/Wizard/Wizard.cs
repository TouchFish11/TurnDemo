using Game.Battle.Context;
using Game.Battle.Skill.Component;
using GameHotUpdate.Battle.Object.Role.Wizard.Skill;

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