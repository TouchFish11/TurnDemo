using System.Collections;
using HotUpdate.Game.Battle.Skill.Base;

namespace HotUpdate.Game.Battle.Skill.Nodes
{
    /// <summary>
    /// 战技点消耗效果
    /// </summary>
    public class SkillPointCastNode : SkillNode
    {
        public SkillPointCastNode(ISkill skill) : base(skill)
        {
        
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override IEnumerator Execute()
        {
            var skillContext = skill.SkillContext;
            // 消耗战斗点数（BP），消耗数值取自技能配置表的f_costBP字段
            skillContext.Caster.Context.ConsumeSkillPoint(skillContext.SkillInfo.f_costBP);
            yield break;
        }
    }
}
