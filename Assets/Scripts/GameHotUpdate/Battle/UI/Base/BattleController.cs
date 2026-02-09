using Game.Battle.Context;
using GameHotUpdate.Battle.UI.MonsterStateUI;
using GameHotUpdate.UI.MVC;

namespace GameHotUpdate.Battle.UI.Base
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
        
        public MonsterStateUIManager MonsterStateUIManager { get; private set; }

        protected override async System.Threading.Tasks.Task OnInit()
        {
            UiInitializer = new BattleUIInitializer(view, model, this);
            BattleUiManager = new BattleUIManager(view, model, this);
            EventProcessor = new BattleEventProcessor(this, BattleUiManager, UiInitializer);

            MonsterStateUIManager = new MonsterStateUIManager();
            
            await System.Threading.Tasks.Task.CompletedTask;
        }

        /// <summary>
        /// 初始化战斗UI
        /// </summary>
        /// <param name="battleContext"></param>
        public async System.Threading.Tasks.Task InitBattleUI(IBattleContext battleContext)
        {
            await UiInitializer.InitPlayerUIs(battleContext.GetAlivePlayerEntitys());
            await UiInitializer.InitMonsterUIs(battleContext.GetAliveMonsterEntitys());
            // 更新战机点数
            await BattleUiManager.UpdateBattlePointCount(battleContext.CurentBattlePointCount, battleContext.MaxBattlePointCount);
            // 注册战斗相关事件
            EventProcessor.RegisterBattleEvents(battleContext.GetEventBus());
        }
    }
}