using Game.Battle.Context;
using Game.Battle.Event;
using Game.Battle.Status;

namespace GameHotUpdate.Battle.Event.UI
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
