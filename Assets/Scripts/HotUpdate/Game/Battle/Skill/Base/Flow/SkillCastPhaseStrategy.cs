using Core.DI;
using Core.Pool;
using HotUpdate.Base.UI;
using HotUpdate.Game.VFX;

namespace HotUpdate.Game.Battle.Skill.Base.Flow
{
    public abstract class SkillCastPhaseStrategy : SkillPhaseStrategy
    {
        [Inject] protected IVFXManager vfxManager;
        [Inject] protected IPoolManager poolManager;
        [Inject] protected IUIService uiService;
    }
}
