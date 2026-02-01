using Game.Battle.Context;
using GameHotUpdate.UI.MVC;

namespace GameHotUpdate.UI.Battle.Base
{
    /// <summary>
    /// 战斗界面控制器
    /// </summary>
    public class BattleController : UIController<BattleView, BattleModel>
    {
        // 依赖注入各子模块
        public BattleUIInitializer UiInitializer { get; private set; }
        
        public BattleEventProcessor EventProcessor { get; private set; }
        
        public BattleUIManager BattleUiManager { get; private set; }

        protected override async System.Threading.Tasks.Task OnInit()
        {
            UiInitializer = new BattleUIInitializer(view, model);
            BattleUiManager = new BattleUIManager(view, model, this);
            EventProcessor = new BattleEventProcessor(this, BattleUiManager, UiInitializer);
            await System.Threading.Tasks.Task.CompletedTask;
        }

        /// <summary>
        /// 初始化战斗UI
        /// </summary>
        /// <param name="battleContext"></param>
        public async System.Threading.Tasks.Task InitBattleUI(IBattleContext battleContext)
        {
            await UiInitializer.InitPlayerUI(battleContext.GetAlivePlayerEntitys());
            await BattleUiManager.UpdateBattlePointCount(battleContext.CurentBattlePointCount, battleContext.MaxBattlePointCount);
            EventProcessor.RegisterBattleEvents(battleContext.GetEventBus());
        }
    }
}