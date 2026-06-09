using System.Threading.Tasks;
using Core.DI;
using Game.Module;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Module;
using HotUpdate.Game.Battle.Damage;
using HotUpdate.Game.Battle.Event;
using HotUpdate.Game.Battle.Inputs;
using HotUpdate.Game.Battle.TargetSelect;
using HotUpdate.Game.Point;

namespace HotUpdate.Game.Battle.Core
{
    /// <summary>
    /// 战斗模块
    /// </summary>
    [ModuleExport(typeof(IBattleModule))]
    public class BattleModule : IBattleModule
    {
        public int Priority => 9;
        
        public void Register()
        {
            DIContainer.BindSingleton<ITargetSelectManager, TargetSelectManager>();
            DIContainer.BindSingleton<IDamageCalcManager, DamageCalcManager>();
            DIContainer.BindSingleton<IBattleInputHandler, BattleInputHandler>();
            DIContainer.BindSingleton<IBattleEventScheduler, BattleEventScheduler>();
            DIContainer.BindSingleton<IBattleCameraManager, BattleCameraManager>();
            DIContainer.BindSingleton<IBattlePointProxy, BattlePointProxy>();
            DIContainer.BindSingleton<IBattleManager, BattleManager>();
        }

        public Task InitModuleAsync()
        {
            return Task.CompletedTask;
        }
    }
}
