using HotUpdate.Game.Battle.Object.Role.Priest.Skill;
using HotUpdate.Game.Battle.Skill.Component;

namespace HotUpdate.Game.Battle.Object.Role.Priest
{
    /// <summary>
    /// 牧师脚本
    /// </summary>
    public class Priest : PlayerObject
    {
        protected override void OnBattleInit()
        {
            GetComponent<SkillComponent>().InitSkills(RoleInfo.f_skillIds, new PriestSkillFactory());
        }
    }
}
