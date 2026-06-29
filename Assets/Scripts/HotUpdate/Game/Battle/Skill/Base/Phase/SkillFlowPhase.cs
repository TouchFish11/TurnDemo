using System.Collections;
using Core.DI;
using HotUpdate.Game.Battle.Core;

namespace HotUpdate.Game.Battle.Skill.Base.Phase
{
    /// <summary>
    /// 技能流程阶段
    /// </summary>
    public abstract class SkillFlowPhase : ISkillFlowPhase
    {
        [Inject] protected BattleCoordinator battleCoordinator;
        protected readonly ISkill skill;
        
        protected SkillContext SkillContext => skill.SkillContext;
        
        protected SkillFlowPhase(ISkill skill)
        {
            this.skill = skill;
        }
        
        public abstract IEnumerator Execute();
    }
}
