using System.Threading.Tasks;
using Core.DI;
using Core.Mono;
using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Damage;
using HotUpdate.Game.Battle.Event;
using HotUpdate.Game.Battle.Inputs;
using HotUpdate.Game.Battle.Object.Monster;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Battle.Operation;
using HotUpdate.Game.Battle.Skill.Conditions;
using HotUpdate.Game.Battle.Skill.Factory;
using HotUpdate.Game.Battle.Statuses;
using HotUpdate.Game.Battle.TargetSelect;
using HotUpdate.Game.Battle.Toughness;
using HotUpdate.Game.Battle.Turn;

namespace HotUpdate.Game.Battle.Core
{
    /// <summary>
    /// 战斗入口
    /// </summary>
    public class BattleEntry
    {
        private static BattleManagerInitializer s_battleManagerInitializer;
        private static IBattleContext s_battleContext;

        /// <summary>
        /// 进入战斗唯一入口
        /// </summary>
        public static async Task StartBattle(BattleStartupParams battleStartupParams)
        {
            // 创建战斗上下文
            s_battleContext = DIContainer.Create<BattleContext>();
            var battleEventBus = DIContainer.Create<BattleEventBus>();
            var battleStateMachine = DIContainer.Create<BattleStateMachine>(parameterValues: s_battleContext);
            s_battleContext.Init(battleEventBus, battleStateMachine);
            
            // 首次构建（只会执行一次，因为 s_battleManagerInitializer 非空）
            if (s_battleManagerInitializer == null)
            {
                BattleRegist();
                s_battleManagerInitializer = DIContainer.Resolve<BattleManagerInitializer>();
            }

            // 同步初始化
            s_battleManagerInitializer.Init(s_battleContext);
            // 异步初始化
            await s_battleManagerInitializer.InitAsync(s_battleContext, battleStartupParams);
            
            // 开始战斗
            s_battleContext.BattleMachine.StartBattle();
        }

        /// <summary>
        /// 结束战斗
        /// </summary>
        public static void EndBattle()
        {
            // 不清空 battleManagerInitializer，单例管理器保留
            s_battleManagerInitializer?.Reset();
            
            // 销毁所有实体 GameObject
            foreach (var entity in s_battleContext.AllBattleEntity)
            {
                entity.Destroy();
                EngineUtility.Destroy(entity.GameObject);
            }
            // 销毁状态机
            s_battleContext.BattleMachine.Dispose();
            // 清空事件总线
            s_battleContext.EventBus.Clear();
            s_battleContext.CleanData();
            s_battleContext = null;
        }
        
        /// <summary>
        /// 战斗注册
        /// </summary>
        private static void BattleRegist()
        {
            DIContainer.BindSingleton<ITargetSelectManager, TargetSelectManager>();
            DIContainer.BindSingleton<IDamageCalcManager, DamageCalcManager>();
            DIContainer.BindSingleton<IBattleInputHandler, BattleInputHandler>();
            DIContainer.BindSingleton<IBattleEventScheduler, BattleEventScheduler>();
            DIContainer.BindSingleton<IBattleCameraManager, BattleCameraManager>();
            DIContainer.BindSingleton<IBattleManager, BattleManager>();
            DIContainer.BindSingleton<IBattleCoordinator, BattleCoordinator>();
            DIContainer.BindSingleton<IBattlePointProxy, BattlePointProxy>();
            DIContainer.BindSingleton<IBattleCommandsController, BattleCommandsController>();
            
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
    }
}
