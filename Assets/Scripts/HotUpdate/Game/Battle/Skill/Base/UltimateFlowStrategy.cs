using System.Collections;
using Core.DI;
using Core.Pool;
using HotUpdate.Base.UI;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.VFX;

namespace HotUpdate.Game.Battle.Skill.Base
{
    public abstract class UltimateFlowStrategy : IUltimateFlowStrategy
    {
        [Inject] protected IUIService uiService;
        [Inject] protected IVFXManager vfxManager;
        [Inject] protected IPoolManager poolManager;
        [Inject] protected BattleCoordinator battleCoordinator;
        
        protected SkillContext skillContext;

        public virtual IEnumerator ExecuteFlow(SkillContext skillContext)
        {
            this.skillContext = skillContext;
            yield return OnExecuteFlow();
        }

        protected abstract IEnumerator OnExecuteFlow();
    }
}
