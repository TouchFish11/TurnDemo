using HotUpdate.Game.Battle.Object.Role.Warrior.Skill;
using HotUpdate.Game.Battle.Skill.Component;

namespace HotUpdate.Game.Battle.Object.Role.Warrior
{
    /// <summary>
    /// 战士脚本
    /// </summary>
    public class Warrior : PlayerObject
    {
        protected override void OnBattleInit()
        {
            GetComponent<SkillComponent>().InitSkills(RoleInfo.f_skillIds, new WarriorSkillFactory());
        }
    }
}
