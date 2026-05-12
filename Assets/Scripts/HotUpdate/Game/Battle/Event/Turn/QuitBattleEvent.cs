using Core.UI.ViewController;
using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Event;

namespace HotUpdate.Game.Battle.Event.Turn
{
    /// <summary>
    /// 战斗退出事件
    /// </summary>
    public class QuitBattleEvent : BattleEvent
    {
        public IuiController BattleUIController { get; }
        
        public QuitBattleEvent(IBattleContext context, IuiController controller) : base(context)
        {
            BattleUIController = controller;
        }
    }
}
