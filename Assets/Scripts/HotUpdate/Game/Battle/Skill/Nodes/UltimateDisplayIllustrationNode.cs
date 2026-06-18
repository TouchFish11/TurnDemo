using System.Collections;
using HotUpdate.Game.Battle.Skill.Base;

namespace HotUpdate.Game.Battle.Skill.Nodes
{
    /// <summary>
    /// 终结技触发时的显示立绘节点
    /// </summary>
    public abstract class UltimateDisplayIllustrationNode : SkillNode
    {
        protected UltimateDisplayIllustrationNode(ISkill skill) : base(skill)
        {
            
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override IEnumerator Execute()
        {
            // 都显示立绘
            yield return battleCoordinator.ExecutePreUltimateCast(skill.SkillContext.Caster, skill.SkillContext.SkillInfo);
        }
    }
}
