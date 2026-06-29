using System.Collections;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Skill.Normal
{
    public class WarriorNormalSkillPreCastPhaseStrategy : SkillPreCastPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            SkillHelper.InitRoleSkillTarget(skill, battleCoordinator);
            
            var skillContext = skill.SkillContext;
            
            // 消耗战斗点数（BP），消耗数值取自技能配置表的f_costBP字段
            skillContext.Caster.Context.ConsumeSkillPoint(skillContext.SkillInfo.f_costBP);
            
            // 该技能不需要初始化投射物
            // ...
            
            var context = skill.SkillContext.Caster.Context;
            context.GetEventBus().TriggerEvent(new PlayerReleaseSkillEvent(context));
            yield break;
        }
    }
}
