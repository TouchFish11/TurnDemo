using System.Collections;
using HotUpdate.Game.Battle.Skill.Base.Flow;

namespace HotUpdate.Game.Battle.Skill.Base.Phase
{
    public class SkillCastPhase : SkillFlowPhase
    {
        private SkillCastPhaseStrategy _skillCastPhaseStrategy;
        
        public SkillCastPhase(ISkill skill) : base(skill)
        {
        
        }

        public override IEnumerator Execute()
        {
            yield return _skillCastPhaseStrategy.Execute();
        }

        public void SetSkillCastPhaseStrategy(SkillCastPhaseStrategy skillCastPhaseStrategy)
        {
            _skillCastPhaseStrategy = skillCastPhaseStrategy;
        }
    }
}
