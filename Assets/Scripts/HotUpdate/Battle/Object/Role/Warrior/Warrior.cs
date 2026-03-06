using HotUpdate.Battle.Context;
using HotUpdate.Battle.Object.Role.Warrior.Skill;
using HotUpdate.Battle.Skill.Component;
using HotUpdate.Core.Battle;

namespace HotUpdate.Battle.Object.Role.Warrior
{
    /// <summary>
    /// 战士脚本
    /// </summary>
    public class Warrior : PlayerObject
    {
        public override void BattleInit(int battleEntityId, IBattleContext context)
        {
            base.BattleInit(battleEntityId, context);
            GetComponent<SkillComponent>().InitSkills(RoleInfo.f_skillIds, new WarriorSkillFactory());
        }
    }
}
