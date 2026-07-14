using System.Collections;
using System.Collections.Generic;
using HotUpdate.Game.Battle.Skill.Base.Phase;

namespace HotUpdate.Game.Battle.Skill.Base.Flow
{
    public interface ISkillFlow
    {
        /// <summary>
        /// 注册阶段
        /// </summary>
        /// <param name="skillFlowPhase"></param>
        void RegisterPhases(List<ISkillFlowPhase> skillFlowPhase);
        
        /// <summary>
        /// 运行技能流程
        /// </summary>
        /// <returns></returns>
        IEnumerator Run();
    }
}
