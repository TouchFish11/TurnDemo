using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Event.UI
{
    /// <summary>
    /// 行动轴排序后事件
    /// </summary>
    public class ActionBarSortPostEvent : BattleEvent
    {
        public ActionBarSortPostEvent(IBattleContext context) : base(context)
        {

        }
    }
}
