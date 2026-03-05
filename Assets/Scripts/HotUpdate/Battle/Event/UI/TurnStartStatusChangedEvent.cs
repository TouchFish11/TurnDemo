using HotUpdate.Battle.Context;
using HotUpdate.Battle.Object;

namespace HotUpdate.Battle.Event.UI
{
    /// <summary>
    /// �غϿ�ʼ״̬�仯�¼�
    /// ���½�ɫ״̬��UI
    /// </summary>
    public class TurnStartStatusChangedEvent : BattleEvent
    {
        public IBattleEntityObject CurrentBattleEntity { get; }

        public TurnStartStatusChangedEvent(IBattleContext context, IBattleEntityObject currentBattleEntity) : base(context)
        {
            CurrentBattleEntity = currentBattleEntity;
        }
    }
}
