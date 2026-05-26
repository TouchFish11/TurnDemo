using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object.Role.Priest.Skill;
using HotUpdate.Game.Battle.Skill.Component;

namespace HotUpdate.Game.Battle.Object.Role.Priest
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
