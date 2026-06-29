using System.Collections;
using HotUpdate.Game.Battle.Skill.Base.Flow;

namespace HotUpdate.Game.Battle.Skill.Base.Phase
{
    /// <summary>
    /// 技能释放结束阶段，等待特效、动画完成、清理相关逻辑
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

        /// <summary>
        /// 设置技能结束逻辑
        /// </summary>
        /// <param name="skillCastEndPhaseStrategy"></param>
        public void SetSkillCastEndPhaseStrategy(SkillCastEndPhaseStrategy skillCastEndPhaseStrategy)
        {
            _skillCastEndPhaseStrategy = skillCastEndPhaseStrategy;
        }
    }
}
