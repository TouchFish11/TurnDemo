using Game.Battle.Context;
using Game.Battle.Skill.Component;
using GameHotUpdate.Battle.Object.Role.Warrior.Skill;

namespace GameHotUpdate.Battle.Object.Role.Warrior
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
