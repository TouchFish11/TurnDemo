using System.Collections.Generic;
using Core.DI;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.Battle.Skill.Base.Phase;

namespace HotUpdate.Game.Battle.Skill.Base
{
    /// <summary>
    /// 技能阶段构建器
    /// </summary>
    public class SkillPhaseBuilder
    {
        private ISkill _skill;
        private readonly List<ISkillFlowPhase> _phases = new();

        public void SetSkill(ISkill skill)
        {
            _skill = skill;
        }
        
        public SkillPhaseBuilder AddMonsterCommonPhase()
        {
            var monsterCommonPhase = DIContainer.Create<MonsterCommonPhase>(parameterValues: _skill);
            _phases.Add(monsterCommonPhase);
            return this;
        }
        
        public SkillPhaseBuilder AddSkillPreCastPhase(SkillPreCastPhaseStrategy strategy)
        {
            var skillPreCastPhase = DIContainer.Create<SkillPreCastPhase>(parameterValues: _skill);
            SetStrategySkill(strategy);
            skillPreCastPhase.SetSkillPreCastPhaseStrategy(strategy);
            _phases.Add(skillPreCastPhase);
            return this;
        }
        
        public SkillPhaseBuilder AddSkillCastPhase(SkillCastPhaseStrategy strategy)
        {
            var skillPreCastPhase = DIContainer.Create<SkillCastPhase>(parameterValues: _skill);
            SetStrategySkill(strategy);
            skillPreCastPhase.SetSkillCastPhaseStrategy(strategy);
            _phases.Add(skillPreCastPhase);
            return this;
        }
        
        public SkillPhaseBuilder AddSkillEventProcessPhase(SkillEventProcessPhaseStrategy strategy)
        {
            var skillEventProcessPhase = DIContainer.Create<SkillEventProcessPhase>(parameterValues: _skill);
            SetStrategySkill(strategy);
            skillEventProcessPhase.SetSkillEventProcessPhaseStrategy(strategy);
            _phases.Add(skillEventProcessPhase);
            return this;
        }
        
        public SkillPhaseBuilder AddSkillCastEndPhase(SkillCastEndPhaseStrategy strategy)
        {
            var skillCastEndPhase = DIContainer.Create<SkillCastEndPhase>(parameterValues: _skill);
            SetStrategySkill(strategy);
            skillCastEndPhase.SetSkillCastEndPhaseStrategy(strategy);
            _phases.Add(skillCastEndPhase);
            return this;
        }

        private void SetStrategySkill(ISkillPhaseStrategy strategy)
        {
            strategy.SetSkill(_skill);
        }
        
        public List<ISkillFlowPhase> Build()
        {
            var list = new List<ISkillFlowPhase>(_phases);
            _phases.Clear();
            return list;
        }
    }
}
