using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Event;
using HotUpdate.Game.Battle.UI.Base;

namespace HotUpdate.Game.Battle.Event.Turn
{
    /// <summary>
    /// 战斗退出事件
    /// </summary>
    public class QuitBattleEvent : BattleEvent
    {
        public BattleController BattleUIController { get; }
        
        public QuitBattleEvent(IBattleContext context, BattleController controller) : base(context)
        {
            BattleUIController = controller;
        }
    }
}
