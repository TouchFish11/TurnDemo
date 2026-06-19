using System.Collections;
using System.Collections.Generic;
using HotUpdate.Game.Battle.Skill.Base.Phase;

namespace HotUpdate.Game.Battle.Skill.Base.Flow
{
    public interface ISkillFlow
    {
        void RegisterPhases(List<ISkillFlowPhase> skillFlowPhase);
        
        IEnumerator Run();
    }
}
