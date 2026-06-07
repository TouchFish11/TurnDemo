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
        public PreparationState(IBattleStateMachine battleStateMachine, IBattleContext context) : base(battleStateMachine, context)
        {
            
        }

        public override void Enter()
        {
            Execute();
        }

        public override async void Execute()
        {
            try
            {
                // 创建战斗界面
                var battleController = await uiService.OpenAsync(EUIPanelId.BattlePanel, E_UILayer.Mid) as IBattleController;
                // 初始化战斗控制器
                battleController.InitBattleController(Context);
            
                // 创建战斗角色、怪物
                var roles = await DIContainer.GetInstance<IBattleManager>().GetTurnCreator().CreateRoles(1,2,3);
                foreach (var battleEntityObject in roles)
                {
                    Context.AddBattleEntity(battleEntityObject);
                    Context.AddSceneRole(battleEntityObject);
                }
            
                var monsters = await DIContainer.GetInstance<IBattleManager>().GetTurnCreator().CreateWave();
                foreach (var battleEntityObject in monsters)
                {
                    Context.AddBattleEntity(battleEntityObject);
                    Context.AddSceneMonster(battleEntityObject);
                }
            
                // 初始化角色战斗点，依赖战斗实体对象创建完成
                DIContainer.GetInstance<IBattlePointProxy>().InitProxy(Context, new List<IBattleEntityObject>(Context.GetAlivePlayerEntitys()));
                // 初始化角色UI
                await battleController.UiInitializer.InitPlayerUIs(Context.GetAlivePlayerEntitys());
                // 初始化怪物UI
                await battleController.UiInitializer.InitMonsterUIs(Context.GetAliveMonsterEntitys());
                // 隐藏怪物uI
                battleController.MonsterStateUIManager.InActiveMonsterUIs();
                // 更新战机点数
                await battleController.BattleUiManager.UpdateBattlePointCount(Context.CurentBattlePointCount, Context.MaxBattlePointCount);
                // 初始化行动顺序
                BattleUtility.InitOrder(Context);
                // 销毁战斗加载界面
                await uiService.CloseAsync(uiService.GetPanel(EUIPanelId.BattleLoadingkPanel).PanelId, true);
            
                BattleStateMachine.ChangeState(EBattlePhase.EnterAnimation);
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(PreparationState)}.{nameof(Execute)}:战斗准备状态执行错误，{e.Message}");
            }
        }

        public override void Exit()
        {
            
        }
    }
}
