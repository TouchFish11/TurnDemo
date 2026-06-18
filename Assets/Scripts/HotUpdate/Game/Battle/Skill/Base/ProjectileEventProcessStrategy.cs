using System;
using Core.DI;
using Core.Time;
using HotUpdate.Game.Battle.Damage;
using HotUpdate.Game.Battle.Status;
using HotUpdate.Game.VFX;

namespace HotUpdate.Game.Battle.Skill.Base
{
    public abstract class ProjectileEventProcessStrategy : IProjectileEventProcessStrategy
    {
        // 状态工厂
        [Inject] protected IStatusFactory statusFactory;
        // 伤害计算管理器
        [Inject] protected IDamageCalcManager damageCalcManager;
        // 特效管理器
        [Inject] protected IVFXManager vfxManager;
        //
        [Inject] protected ITimerManager timerManager;
        
    }
}
