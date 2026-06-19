using System.Collections;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Skill.Base.Phase
{
    public class SkillEventProcessPhase : SkillFlowPhase
    {
        private SkillEventProcessPhaseStrategy _skillEventProcessPhaseStrategy;
        
        public SkillEventProcessPhase(ISkill skill) : base(skill)
        {
            
        }

        public override IEnumerator Execute()
        {
            _skillEventProcessPhaseStrategy.Reset();
            SkillContext.Projectile.OnTrigger += _skillEventProcessPhaseStrategy.ProcessEvent;
            yield return new WaitWhile(() => _skillEventProcessPhaseStrategy.IsProcessing);
            SkillContext.Projectile.OnTrigger -= _skillEventProcessPhaseStrategy.ProcessEvent;
        }

        public void SetSkillEventProcessPhaseStrategy(SkillEventProcessPhaseStrategy skillEventProcessPhaseStrategy)
        {
            _skillEventProcessPhaseStrategy = skillEventProcessPhaseStrategy;
        }
    }
}
