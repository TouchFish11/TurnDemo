using System.Collections;
using Core.DI;
using HotUpdate.Game.Battle.Core;

namespace HotUpdate.Game.Battle.Skill.Base
{
    /// <summary>
    /// 技能效果
    /// </summary>
    public abstract class SkillNode : ISkillNode
    {
        [Inject] protected BattleCoordinator battleCoordinator;
        protected readonly ISkill skill;
        
        protected SkillContext SkillContext => skill.SkillContext;
        
        protected SkillNode(ISkill skill)
        {
            this.skill = skill;
        }

        public abstract bool CanExecute();
        
        public abstract IEnumerator Execute();
    }
}
