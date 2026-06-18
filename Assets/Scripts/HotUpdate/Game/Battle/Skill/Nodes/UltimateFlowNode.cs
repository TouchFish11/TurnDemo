using System.Collections;
using HotUpdate.Game.Battle.Skill.Base;

namespace HotUpdate.Game.Battle.Skill.Nodes
{
    /// <summary>
    /// 终结技流节点
    /// </summary>
    public class UltimateFlowNode : SkillNode
    {
        private IUltimateFlowStrategy _ultimateFlowStrategy;
        
        public UltimateFlowNode(ISkill skill) : base(skill)
        {
        
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override IEnumerator Execute()
        {
            yield return _ultimateFlowStrategy.ExecuteFlow(SkillContext);
        }

        public void SetUltimateFlowStrategy(IUltimateFlowStrategy ultimateFlowStrategy)
        {
            _ultimateFlowStrategy = ultimateFlowStrategy;
        }
    }
}
