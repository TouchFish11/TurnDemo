using Core.UI.ViewController;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.UI;
using HotUpdate.UI.Battle.MonsterStateUI;

namespace HotUpdate.UI.Battle.Base
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 战斗界面控制器
    /// </summary>
    public class BattleController : UIController<BattleView>, IBattleController
    {
        public IBattleUIInitializer UiInitializer { get; private set; }
        
        public IBattleEventProcessor EventProcessor { get; private set; }
        
        public IBattleUIManager BattleUiManager { get; private set; }
        
        public IMonsterStateUIManager MonsterStateUIManager { get; private set; }
        
        protected override Task OnInit()
        {
            return Task.CompletedTask;
        }

        protected override Task OnActive()
        {
            UiInitializer = new BattleUIInitializer(view, this);
            BattleUiManager = new BattleUIManager(view, this);
            EventProcessor = new BattleEventProcessor(this, BattleUiManager, UiInitializer);
            MonsterStateUIManager = new MonsterStateUIManager();
            return Task.CompletedTask;
        }

        protected override Task OnInactivate()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 初始化战斗控制器
        /// </summary>
        /// <param name="battleContext"></param>
        public void InitBattleController(IBattleContext battleContext)
        {
            // 注册战斗相关事件
            EventProcessor.RegisterBattleEvents(battleContext.GetEventBus());
        }
    }
}