using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Event;
using HotUpdate.Base.Battle.Object;

namespace HotUpdate.Game.Battle.Event.Turn
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
