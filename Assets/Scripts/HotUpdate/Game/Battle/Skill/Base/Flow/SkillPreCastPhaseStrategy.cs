using Core.DI;
using Core.Pool;

namespace HotUpdate.Game.Battle.Skill.Base.Flow
{
    /// <summary>
    /// 技能前摇阶段策略
    /// </summary>
    public abstract class SkillPreCastPhaseStrategy : SkillPhaseStrategy
    {
        [Inject] protected IPoolManager poolManager;
    }
}
