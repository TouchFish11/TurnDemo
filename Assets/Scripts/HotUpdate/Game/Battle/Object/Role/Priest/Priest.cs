using Core.DI;
using HotUpdate.Game.Battle.Object.Role.Priest.Skill;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;

namespace HotUpdate.Game.Battle.Object.Role.Priest
{
    /// <summary>
    /// 牧师脚本
    /// </summary>
    public class Priest : PlayerObject
    {
        protected override void OnBattleInit()
        {
            var skillComponent = GetComponent<ISkillComponent>();
            var core = DIContainer.Create<SkillComponentCore>();
            core.InitSkill(RoleInfo.f_skillIds, DIContainer.Create<PriestSkillFactory>());
            skillComponent.InitSkill(this, TODO, TODO);
        }
    }
}
