using GameHotUpdate.Battle.Context;
using GameHotUpdate.Battle.Object.Role.Priest.Skill;
using GameHotUpdate.Battle.Skill.Component;

namespace GameHotUpdate.Battle.Object.Role.Priest
{
    /// <summary>
    /// 牧师脚本
    /// </summary>
    public class Priest : PlayerObject
    {
        public override void BattleInit(int battleEntityId, IBattleContext context)
        {
            base.BattleInit(battleEntityId, context);
            GetComponent<SkillComponent>().InitSkills(RoleInfo.f_skillIds, new PriestSkillFactory());
        }
    }
}
