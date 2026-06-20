using System.Collections;
using HotUpdate.Game.Battle.Skill.Base.Flow;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Skill.Normal
{
    public class WarriorNormalSkillPreCastPhaseStrategy : SkillPreCastPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            var skillContext = skill.SkillContext;
            // 根据技能配置和选择策略，筛选出技能作用的目标
            battleCoordinator.SetSelectSkillInfo(skillContext.SkillInfo);
            battleCoordinator.SelectTargets(skillContext.Caster, skillContext.TargetSelectStrategy);
            // TODO；暂时这样写
            battleCoordinator.InitSkillTarget(skill);
            
            // 消耗战斗点数（BP），消耗数值取自技能配置表的f_costBP字段
            skillContext.Caster.Context.ConsumeSkillPoint(skillContext.SkillInfo.f_costBP);
            
            // 该技能不需要初始化投射物
            yield break;
        }
    }
}
