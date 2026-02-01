using Core.UI.MVC;
using Game.Battle.Context;
using Game.Battle.Event;

namespace GameHotUpdate.Battle.Event.Turn
{
    /// <summary>
    /// �˳�ս���¼�
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
