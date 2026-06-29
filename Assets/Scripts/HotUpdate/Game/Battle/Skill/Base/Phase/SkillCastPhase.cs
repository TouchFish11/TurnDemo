using System.Collections;
using HotUpdate.Game.Battle.Skill.Base.Flow;

namespace HotUpdate.Game.Battle.Skill.Base.Phase
{
    /// <summary>
    /// 技能释放核心阶段
    /// </summary>
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

        /// <summary>
        /// 设置技能释放策略
        /// </summary>
        /// <param name="skillCastPhaseStrategy"></param>
        public void SetSkillCastPhaseStrategy(SkillCastPhaseStrategy skillCastPhaseStrategy)
        {
            _skillCastPhaseStrategy = skillCastPhaseStrategy;
        }
    }
}
