using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Event;
using HotUpdate.Base.Battle.Object;

namespace HotUpdate.Game.Battle.Event.UI
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
