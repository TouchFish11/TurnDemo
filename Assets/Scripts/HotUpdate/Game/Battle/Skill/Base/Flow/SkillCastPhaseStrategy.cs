using System.Collections;
using Core.DI;
using HotUpdate.Game.VFX;

namespace HotUpdate.Game.Battle.Skill.Base.Flow
{
    public abstract class SkillCastPhaseStrategy : SkillPhaseStrategy
    {
        [Inject] protected IVFXManager vfxManager;
    }
}
