using System.Collections;
using HotUpdate.Game.Battle.Skill.Base;

namespace HotUpdate.Game.Battle.Skill.Nodes
{
    /// <summary>
    /// 目标选择效果，设置目标管理器
    /// </summary>
    public class TargetSelectNode : SkillNode
    {
        public TargetSelectNode(ISkill skill) : base(skill)
        {
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override IEnumerator Execute()
        {
            var skillContext = skill.SkillContext;
            // 根据技能配置和选择策略，筛选出技能作用的目标
            battleCoordinator.SetSelectSkillInfo(skillContext.SkillInfo);
            battleCoordinator.SelectTargets(skillContext.Caster, skillContext.TargetSelectStrategy);
            // TODO；暂时这样写
            battleCoordinator.InitSkillTarget(skill);
            yield break;
        }
    }
}
