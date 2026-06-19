using System.Collections;
using HotUpdate.Game.Battle.Skill.Base.Flow;

namespace HotUpdate.Game.Battle.Skill.Base.Phase
{
    /// <summary>
    /// 技能释放前摇阶段
    /// </summary>
    public class SkillPreCastPhase : SkillFlowPhase
    {
        private SkillPreCastPhaseStrategy _skillPreCastPhaseStrategy;
        
        public SkillPreCastPhase(ISkill skill) : base(skill)
        {
        
        }

        public override IEnumerator Execute()
        {
            yield return _skillPreCastPhaseStrategy.Execute();
        }

        public void SetSkillPreCastPhaseStrategy(SkillPreCastPhaseStrategy skillPreCastPhaseStrategy)
        {
            _skillPreCastPhaseStrategy = skillPreCastPhaseStrategy;
        }
    }
}
