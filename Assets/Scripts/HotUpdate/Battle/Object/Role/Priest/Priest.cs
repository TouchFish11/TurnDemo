using HotUpdate.Battle.Context;
using HotUpdate.Battle.Object.Role.Priest.Skill;
using HotUpdate.Battle.Skill.Component;
using HotUpdate.Core.Battle;

namespace HotUpdate.Battle.Object.Role.Priest
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
