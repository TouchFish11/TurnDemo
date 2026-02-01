using Game.Battle.Context;
using Game.Battle.Event;
using Game.Battle.Objects;

namespace GameHotUpdate.Battle.Event.Turn
{
    /// <summary>
    /// ��ɫ�ж������¼�
    /// </summary>
    public class TurnEndEvent : BattleEvent
    {
        /// <summary>
        /// �ս����ж���ʵ��
        /// </summary>
        public IBattleEntityObject CurrentBattleEntity { get; }

        public TurnEndEvent(IBattleContext context, IBattleEntityObject currentChar) : base(context)
        {
            CurrentBattleEntity = currentChar;
        }
    }
}
