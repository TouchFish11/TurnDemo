using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Event.UI
{
    /// <summary>
    /// 行动格子状态关闭UI事件
    /// </summary>
    public class ActionGridClickCloseEvent : BattleEvent
    {
        public ActionGridClickCloseEvent(IBattleContext context) : base(context)
        {
        
        }
    }
}
