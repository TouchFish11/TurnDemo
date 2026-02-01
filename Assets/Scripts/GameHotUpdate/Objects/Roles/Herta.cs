using Game.Battle.Context;
using Game.Battle.Skill.Component;
using GameHotUpdate.Skill.Fatory.Roles;

namespace GameHotUpdate.Objects.Roles
{
    /// <summary>
    /// Herta��ɫ��
    /// </summary>
    public class Herta : PlayerObject
    {
        public override void BattleInit(int battleEntityId, IBattleContext context)
        {
            base.BattleInit(battleEntityId, context);
            GetComponent<SkillComponent>().InitSkills(RoleInfo.f_skillIds, new HertaSkillFactory());
        }
    }
}