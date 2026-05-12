using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Event;
using HotUpdate.Base.Battle.Object;

namespace HotUpdate.Game.Battle.Event.Turn
{
    public class TurnStartEvent : BattleEvent
    {
        /// <summary>
        /// ���غ��ж���ʵ��
        /// </summary>
        public IBattleEntityObject CurrentBattleEntity { get; private set; } 

        public TurnStartEvent(IBattleContext context, IBattleEntityObject currentChar) : base(context)
        {
            CurrentBattleEntity = currentChar;
        }
    }
}
