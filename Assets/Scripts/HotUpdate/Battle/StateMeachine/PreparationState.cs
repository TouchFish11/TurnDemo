using System;
using System.Collections.Generic;
using Core.Log;
using Core.Service;
using Core.UI;
using HotUpdate.Battle.UI.Base;
using HotUpdate.Battle.Utility;
using HotUpdate.Common;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.Battle.Point;
using HotUpdate.Core.Battle.Turn;
using HotUpdate.Core.MVC;

namespace HotUpdate.Battle.StateMeachine
{
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
                var battleController = await ServiceLocator.Get<IUIManager>().CreateViewAsync<BattleView, BattleModel,BattleController>(AbKeyCollection.Ui, E_UILayer.Mid, ResKeyCollection.BattleView);
                // 初始化战斗控制器
                battleController.InitBattleController(Context);
            
                // 创建战斗角色、怪物
                var roles = await ServiceLocator.Get<IBattleManager>().GetTurnCreator().CreateRoles(1,2,3);
                foreach (var battleEntityObject in roles)
                {
                    Context.AddBattleEntity(battleEntityObject);
                    Context.AddSceneRole(battleEntityObject);
                }
            
                var monsters = await ServiceLocator.Get<IBattleManager>().GetTurnCreator().CreateWave();
                foreach (var battleEntityObject in monsters)
                {
                    Context.AddBattleEntity(battleEntityObject);
                    Context.AddSceneMonster(battleEntityObject);
                }
            
                // 初始化角色战斗点，依赖战斗实体对象创建完成
                ServiceLocator.Get<IBattlePointProxy>().InitProxy(Context, new List<IBattleEntityObject>(Context.GetAlivePlayerEntitys()));
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
                var loadingController = ServiceLocator.Get<IUIManager>().GetController<IBattleLoadingController>();
                ServiceLocator.Get<IUIManager>().DestroyView(AbKeyCollection.Ui, loadingController);
            
                BattleStateMachine.ChangeState(EBattlePhase.EnterAnimation);
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(PreparationState)}.{nameof(Execute)} : {e.Message}，{e.StackTrace}");
            }
        }

        public override void Exit()
        {
            
        }
    }
}
