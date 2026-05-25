using Core.UI.ViewController;
using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.UI
{
    public interface IBattleController : IuiController
    {
        IBattleUIInitializer UiInitializer { get; }
        
        IBattleEventProcessor EventProcessor { get; }
        
        IBattleUIManager BattleUiManager { get; }
        
        IMonsterStateUIManager MonsterStateUIManager { get; }

        /// <summary>
        /// 初始化战斗控制器
        /// </summary>
        /// <param name="battleContext"></param>
        void InitBattleController(IBattleContext battleContext);
    }
}
