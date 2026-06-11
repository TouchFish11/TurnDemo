using System;
using System.Collections.Generic;
using Core.DI;
using Core.Log;
using Core.UI;
using HotUpdate.Base;
using HotUpdate.Base.UI;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Turn;
using HotUpdate.Game.Battle.UI;
using HotUpdate.Game.Battle.Utility;
using HotUpdate.Game.Point;

namespace HotUpdate.Game.Battle.StateMeachine
{
    /// <summary>
    /// 战斗准备状态
    /// </summary>
    public class PreparationState : BattleState
    {
        [Inject] private IBattleManager _battleManager;
        [Inject] private IBattlePointProxy _battlePointProxy;
        
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
                // 创建，缓存战斗角色、怪物
                var roles = await _battleManager.GetWaveCreator().CreateRoles(1,2,3);
                foreach (var battleEntityObject in roles)
                {
                    Context.AddBattleEntity(battleEntityObject);
                    Context.AddSceneRole(battleEntityObject);
                }

                var monsters = await _battleManager.GetWaveCreator().CreateWave();
                foreach (var battleEntityObject in monsters)
                {
                    Context.AddBattleEntity(battleEntityObject);
                    Context.AddSceneMonster(battleEntityObject);
                }

                // 初始化角色战斗点，依赖战斗实体对象创建完成
                _battlePointProxy.InitProxy(Context, new List<IBattleEntityObject>(Context.GetAlivePlayerEntitys()));
                // 初始化角色UI
                await battleController.UiInitializer.InitPlayerUIs(Context.GetAlivePlayerEntitys());
                // 初始化怪物UI
                await battleController.UiInitializer.InitMonsterUIs(Context.GetAliveMonsterEntitys());
                // 隐藏怪物uI
                battleController.MonsterStateUIManager.InActiveMonsterUIs();
                // 更新战技点UI
                await battleController.BattleUiManager.UpdateBattlePointCount(Context.CurentBattlePointCount, Context.MaxBattlePointCount);
                // 初始化行动顺序
                BattleUtility.InitOrder(Context);
                // 销毁战斗加载界面
                await uiService.CloseAsync(uiService.GetPanel(EUIPanelId.BattleLoadingkPanel).PanelId, true);

                BattleStateMachine.ChangeState(EBattlePhase.EnterAnimation);
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(PreparationState)}.{nameof(Enter)}:战斗准备状态执行错误，{e.Message}");
            }
        }

        public override void Exit()
        {
            
        }

        protected override void OnDispose()
        {
            
        }
    }
}
