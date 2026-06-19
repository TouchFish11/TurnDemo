using System.Collections;
using HotUpdate.Game.Battle.Skill.Base.Flow;

namespace HotUpdate.Game.Battle.Skill.Base.Phase
{
    /// <summary>
    /// 技能释放收尾阶段
    /// </summary>
    public class SkillCastEndPhase : SkillFlowPhase
    {
        private SkillCastEndPhaseStrategy _skillCastEndPhaseStrategy;
        
        public SkillCastEndPhase(ISkill skill) : base(skill)
        {
            
        }

        public override IEnumerator Execute()
        {
            yield return _skillCastEndPhaseStrategy.Execute();
        }

        public void SetSkillCastEndPhaseStrategy(SkillCastEndPhaseStrategy skillCastEndPhaseStrategy)
        {
            _skillCastEndPhaseStrategy = skillCastEndPhaseStrategy;
        }
    }
}
