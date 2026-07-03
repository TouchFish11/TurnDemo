using System.Collections;
using Core.DI;
using Core.Mono;
using Core.Pool;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.VFX;

namespace HotUpdate.Game.Battle.Object
{
    /// <summary>
    /// 死亡处理器
    /// </summary>
    public abstract class DeathHandler : IDeathHandler
    {
        [Inject] protected IVFXManager vfxManager;
        [Inject] protected IPoolManager poolManager;
        [Inject] protected IMonoAdapter monoAdapter;
 
        protected IBattleEntityObject battleEntityObject;
        
        protected IBattleContext Context => battleEntityObject.Context;

        public void InitEntity(IBattleEntityObject entity)
        {   
            battleEntityObject = entity;
        }

        public IEnumerator HandleDeath()
        {
            yield return OnHandle();
            battleEntityObject.Destroy();
        }

        protected abstract IEnumerator OnHandle();
    }
}
