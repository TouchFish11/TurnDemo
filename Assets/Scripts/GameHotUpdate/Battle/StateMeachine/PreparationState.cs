using Core.Config;
using Core.Service;
using Core.UI;
using Game.Battle.Context;
using Game.Battle.Enum;
using Game.Battle.Turn;
using GameHotUpdate.Battle.Event.General;
using GameHotUpdate.Battle.Event.UI;
using GameHotUpdate.Battle.UI.Base;
using GameHotUpdate.Property;
using GameHotUpdate.UI.Loading.Battle;

namespace GameHotUpdate.Battle.StateMeachine
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
            // 创建战斗界面
            var battleController = await ServiceLocator.Get<IUIManager>().CreateViewAsync<BattleView, BattleModel,BattleController>(E_UILayer.Mid, ResKeyCollection.BattleView);
            // 初始化战斗控制器
            battleController.InitBattleController(Context);
            // 初始化角色UI
            await battleController.UiInitializer.InitPlayerUIs(Context.GetAlivePlayerEntitys());
            // 初始化怪物UI
            await battleController.UiInitializer.InitMonsterUIs(Context.GetAliveMonsterEntitys());
            // 更新战机点数
            await battleController.BattleUiManager.UpdateBattlePointCount(Context.CurentBattlePointCount, Context.MaxBattlePointCount);
            // 失活战斗界面
            ServiceLocator.Get<IUIManager>().SetViewActive(battleController, false);
            // 销毁战斗加载界面
            var loadingController = ServiceLocator.Get<IUIManager>().GetController<BattleLoadingController>();
            ServiceLocator.Get<IUIManager>().DestroyView(loadingController);
            
            BattleStateMachine.ChangeState(EBattlePhase.EnterAnimation);
        }

        public override void Exit()
        {
            
        }
    }
}
