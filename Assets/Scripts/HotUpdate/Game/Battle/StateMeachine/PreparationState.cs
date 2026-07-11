using System;
using Core.DI;
using Core.Log;
using Core.Mono;
using Core.Tasks;
using Core.UI;
using HotUpdate.Base.UI;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Turn;
using HotUpdate.Game.Battle.UI;

namespace HotUpdate.Game.Battle.StateMeachine
{
    /// <summary>
    /// 战斗准备状态
    /// </summary>
    public class PreparationState : BattleState
    {
        [Inject] private IBattleManager _battleManager;
        [Inject] private IBattleCoordinator _battleCoordinator;
        [Inject] private IMonoAdapter _monoAdapter;
        
        public PreparationState(IBattleStateMachine battleStateMachine, IBattleContext context) : base(battleStateMachine, context)
        {
            
        }

        public override async void Enter()
        {
            try
            {
                // 初始化战斗界面
                var battleController = (IBattleController)await uiService.OpenAsync(EUIPanelId.BattlePanel, E_UILayer.Mid);
                battleController.InitBattleController(Context);

                // TODO：暂时写死，可根据配置优化
                // 创建并缓存战斗角色
                await _battleManager.BattleService.CreatePlayerRoles(1,2,3);
                // 初始化战斗协调器
                _battleCoordinator.Init(Context);
                // 初始化角色UI
                await battleController.UiInitializer.InitPlayerUIs(Context.GetAlivePlayerEntitys());
                // 更新战技点UI
                await battleController.BattleUiManager.UpdateBattlePointCount(Context.CurentBattlePointCount, Context.MaxBattlePointCount);
                // 更新波次
                await TaskUtility.WaitForCoroutine(_battleManager.BattleService.UpdateWave(), _monoAdapter);
                // 隐藏加载界面
                await uiService.CloseAsync(uiService.GetPanel(EUIPanelId.BattleLoadingkPanel).PanelId, true);

                BattleStateMachine.ChangeState(EBattlePhase.EnterAnimation);
            }
            catch (Exception e)
            {
                Logger.LogError(ELogTags.Battle, $"{nameof(PreparationState)}: Battle readiness execution error,{e.Message}");
            }
        }

        public override void Exit()
        {
            
        }

        protected override void OnDispose()
        {
            _battleManager = null;
            _battleCoordinator = null;
            _monoAdapter = null;
        }
    }
}
