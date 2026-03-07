using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Event;
using HotUpdate.Core.Battle.Object;

namespace HotUpdate.Battle.Event.Turn
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
