using System.Collections;
using System.Collections.Generic;
using HotUpdate.Game.Battle.Skill.Base.Phase;

namespace HotUpdate.Game.Battle.Skill.Base.Flow
{
    /// <summary>
    /// 技能流对象基类，普通技能、终结技技能、怪物技能
    /// </summary>
    public sealed class SkillFlow : ISkillFlow
    {
        private List<ISkillFlowPhase> _skillFlowPhases;

        public void RegisterPhases(List<ISkillFlowPhase> skillFlowPhase)
        {
            _skillFlowPhases = skillFlowPhase;
        }
        
        public IEnumerator Run()
        {
            foreach (var flowPhase in _skillFlowPhases)
            {
                yield return flowPhase.Execute();
            }
        }
    }
}
