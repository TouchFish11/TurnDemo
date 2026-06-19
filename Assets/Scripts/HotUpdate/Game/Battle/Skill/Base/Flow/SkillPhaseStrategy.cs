using System.Collections;
using Core.DI;
using HotUpdate.Game.Battle.Core;

namespace HotUpdate.Game.Battle.Skill.Base.Flow
{
    /// <summary>
    /// 技能阶段策略
    /// </summary>
    public abstract class SkillPhaseStrategy : ISkillPhaseStrategy
    {
        [Inject] protected BattleCoordinator battleCoordinator;

        protected ISkill skill;

        protected SkillContext SkillContext => skill.SkillContext;
        
        public void SetSkill(ISkill skill)
        {
            this.skill = skill;
        }

        public abstract IEnumerator Execute();
    }
}
