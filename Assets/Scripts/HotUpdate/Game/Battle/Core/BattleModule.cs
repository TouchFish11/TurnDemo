using System.Threading.Tasks;
using Core.DI;
using Game.Module;
using HotUpdate.Base.Module;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Damage;
using HotUpdate.Game.Battle.Event;
using HotUpdate.Game.Battle.Inputs;
using HotUpdate.Game.Battle.Object.Monster;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Battle.Operation;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Conditions;
using HotUpdate.Game.Battle.Skill.Factory;
using HotUpdate.Game.Battle.Statuses;
using HotUpdate.Game.Battle.TargetSelect;
using HotUpdate.Game.Battle.Toughness;

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
            DIContainer.BindSingleton<IBattleCoordinator, BattleCoordinator>();
            DIContainer.BindSingleton<IBattlePointProxy, BattlePointProxy>();
            
            DIContainer.BindSingleton<ICastSkillConditionFactory, CastSkillConditionFactory>();
            DIContainer.BindSingleton<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>();
            DIContainer.BindSingleton<ISkillKeyUIDataProviderFactory, SkillKeyUIDataProviderFactory>();
            DIContainer.BindSingleton<IStatusFactory, StatusFactory>();
            DIContainer.BindSingleton<ITargetSelectStrategyFactory, TargetSelectStrategyFactory>();
            DIContainer.BindSingleton<IToughnessStrategyFactory, ToughnessStrategyFactory>();
            DIContainer.BindSingleton<IMonsterFactory, MonsterFactory>();
            DIContainer.BindSingleton<IRoleFactory, RoleFactory>();
            
            DIContainer.BindType<IBattleContext, BattleContext>();
        }

        public Task InitModuleAsync()
        {
            DIContainer.Create<BattleManagerInitializer>();
            return Task.CompletedTask;
        }
    }
}
