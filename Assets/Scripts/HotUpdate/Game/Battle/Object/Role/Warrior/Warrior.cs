using Core.DI;
using HotUpdate.Game.Battle.Object.Role.Warrior.Skill;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;

namespace HotUpdate.Game.Battle.Object.Role.Warrior
{
    /// <summary>
    /// 战士脚本
    /// </summary>
    public class Warrior : PlayerObject
    {
        protected override void OnBattleInit()
        {
            var skillComponent = GetComponent<ISkillComponent>();
            var core = DIContainer.Create<SkillComponentCore>();
            core.Init(skillComponent, RoleInfo.f_skillIds, DIContainer.Create<WarriorSkillFactory>());
            skillComponent.InitSkill(this, core);
        }
    }
}
