using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Statuses;

namespace HotUpdate.Game.Battle.Event.UI
{
    /// <summary>
    /// 状态添加事件
    /// </summary>
    public class StatusAddedEvent : BattleEvent
    {
        public IStatus NewStatus { get; }

        public StatusAddedEvent(IBattleContext context, IStatus newStatus) : base(context)
        {
            NewStatus = newStatus;
        }
    }
}
