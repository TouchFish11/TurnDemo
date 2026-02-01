using Game.Battle.Context;
using Game.Battle.Event;
using Game.Battle.Objects;

namespace GameHotUpdate.Battle.Event.UI
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
