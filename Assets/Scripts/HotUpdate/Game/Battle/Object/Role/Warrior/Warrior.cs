using HotUpdate.Base.Battle;
using HotUpdate.Game.Battle.Object.Role.Warrior.Skill;
using HotUpdate.Game.Battle.Skill.Component;

namespace HotUpdate.Game.Battle.Object.Role.Warrior
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
