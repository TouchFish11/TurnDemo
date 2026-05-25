using HotUpdate.Base;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.UI;

namespace HotUpdate.Game.Battle.Event.Turn
{
    /// <summary>
    /// 战斗退出事件
    /// </summary>
    public class QuitBattleEvent : BattleEvent
    {
        public IBattleController BattleUIController { get; }
        
        public QuitBattleEvent(IBattleContext context, IBattleController controller) : base(context)
        {
            BattleUIController = controller;
        }
    }
}
