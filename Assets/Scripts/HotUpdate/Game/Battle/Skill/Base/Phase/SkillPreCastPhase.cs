using System.Collections;
using HotUpdate.Game.Battle.Skill.Base.Flow;

namespace HotUpdate.Game.Battle.Skill.Base.Phase
{
    /// <summary>
    /// 技能释放前阶段，处理前摇、弹射物初始化等其它相关业务逻辑
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

        /// <summary>
        /// 设置技能释放前逻辑策略
        /// </summary>
        /// <param name="skillPreCastPhaseStrategy"></param>
        public void SetSkillPreCastPhaseStrategy(SkillPreCastPhaseStrategy skillPreCastPhaseStrategy)
        {
            _skillPreCastPhaseStrategy = skillPreCastPhaseStrategy;
        }
    }
}
