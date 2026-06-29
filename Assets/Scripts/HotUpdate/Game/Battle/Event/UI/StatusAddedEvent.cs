using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Statuses;

namespace HotUpdate.Game.Battle.Event.UI
{
    /// <summary>
    /// ״̬�������¼�
    /// ������ʾ����״̬��Buff���ı�
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
