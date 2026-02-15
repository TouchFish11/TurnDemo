using Game.Battle.Context;
using Game.Battle.Skill.Component;
using GameHotUpdate.Battle.Object.Role.Priest.Skill;

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
