using System.Collections;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Skill.Base.Phase
{
    /// <summary>
    /// 技能事件处理阶段
    /// </summary>
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
            yield return new WaitWhile(() => SkillContext.VFXInfo.IsAlive);
            SkillContext.Projectile.OnTrigger -= _skillEventProcessPhaseStrategy.ProcessEvent;
        }

        /// <summary>
        /// 设置技能事件处理策略
        /// </summary>
        /// <param name="skillEventProcessPhaseStrategy"></param>
        public void SetSkillEventProcessPhaseStrategy(SkillEventProcessPhaseStrategy skillEventProcessPhaseStrategy)
        {
            _skillEventProcessPhaseStrategy = skillEventProcessPhaseStrategy;
        }
    }
}
