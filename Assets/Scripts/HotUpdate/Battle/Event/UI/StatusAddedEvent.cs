using HotUpdate.Battle.Context;
using HotUpdate.Battle.Status;

namespace HotUpdate.Battle.Event.UI
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
