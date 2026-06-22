using System.Threading.Tasks;
using Core.DI;
using Game.Module;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Module;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Damage;
using HotUpdate.Game.Battle.Event;
using HotUpdate.Game.Battle.Inputs;
using HotUpdate.Game.Battle.TargetSelect;
using HotUpdate.Game.Battle.UI;

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
            DIContainer.BindSingleton<IBattleManager, BattleManager>();
            DIContainer.BindSingleton<ISkillKeyUIDataProviderFactory, SkillKeyUIDataProviderFactory>();
            DIContainer.BindType<IBattleContext, BattleContext>();
            
            // TODO：暂时写在这个方法中
            DIContainer.Create<BattleCoordinator>(true);
            DIContainer.Create<BattlePointProxy>(true);
        }

        public Task InitModuleAsync()
        {
            return Task.CompletedTask;
        }
    }
}
