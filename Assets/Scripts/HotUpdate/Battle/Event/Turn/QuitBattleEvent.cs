using Core.UI.MVC;
using HotUpdate.Battle.Context;

namespace HotUpdate.Battle.Event.Turn
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
