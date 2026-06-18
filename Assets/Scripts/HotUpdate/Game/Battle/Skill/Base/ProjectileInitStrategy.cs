using Core.DI;
using Core.Pool;

namespace HotUpdate.Game.Battle.Skill.Base
{
    public abstract class ProjectileInitStrategy : IProjectileInitStrategy
    {
        [Inject] protected IPoolManager poolManager;
    }
}
