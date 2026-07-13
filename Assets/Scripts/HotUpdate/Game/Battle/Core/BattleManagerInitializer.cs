using System.Threading.Tasks;
using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Damage;
using HotUpdate.Game.Battle.Event;
using HotUpdate.Game.Battle.Inputs;
using HotUpdate.Game.Battle.TargetSelect;

namespace HotUpdate.Game.Battle.Core
{
    /// <summary>
    /// 战斗管理器初始化器
    /// </summary>
    public class BattleManagerInitializer
    {
        private readonly ITargetSelectManager _targetSelectManager;
        private readonly IDamageCalcManager _damageCalcManager;
        private readonly IBattleInputHandler _battleInputHandler;
        private readonly IBattleEventScheduler _battleEventScheduler;
        private readonly IBattleCameraManager _battleCameraManager;
        private readonly IBattleManager _battleManager;
        private readonly IBattleCoordinator _battleCoordinator;
        private readonly IBattlePointProxy _battlePointProxy;
        private readonly IBattleCommandsController _battleCommandsController;
        
        private BattleManagerInitializer
        (
            ITargetSelectManager targetSelectManager,
            IDamageCalcManager damageCalcManager,
            IBattleInputHandler battleInputHandler,
            IBattleEventScheduler battleEventScheduler,
            IBattleCameraManager battleCameraManager,
            IBattleManager battleManager,
            IBattleCoordinator battleCoordinator,
            IBattlePointProxy battlePointProxy,
            IBattleCommandsController battleCommandsController
            )
        {
            _targetSelectManager = targetSelectManager;
            _damageCalcManager = damageCalcManager;
            _battleInputHandler = battleInputHandler;
            _battleEventScheduler = battleEventScheduler;
            _battleCameraManager = battleCameraManager;
            _battleManager = battleManager;
            _battleCoordinator = battleCoordinator;
            _battlePointProxy = battlePointProxy;
            _battleCommandsController = battleCommandsController;
        }

        public void Init(IBattleContext context)
        {
            _targetSelectManager.Init(context);
            _damageCalcManager.Init(context);
            _battleInputHandler.Init(context);
            _battleEventScheduler.Init(context);
            _battleCameraManager.Init(context);
            _battleCommandsController.Init(context);
        }
        
        public async Task InitAsync(IBattleContext context, BattleStartupParams startupParams)
        {
            await _battleManager.Init(context, startupParams);
        }

        public void Reset()
        {
            _targetSelectManager.Reset();
            _damageCalcManager.Reset();
            _battleInputHandler.Reset();
            _battleEventScheduler.Reset();
            _battleCameraManager.Reset();
            _battleManager.Reset();
            _battleCoordinator.Reset();
            _battlePointProxy.Reset();
            _battleCommandsController.Reset();
        }
    }
}
