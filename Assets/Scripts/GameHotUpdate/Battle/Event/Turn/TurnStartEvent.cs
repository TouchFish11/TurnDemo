using GameHotUpdate.Battle.Context;
using GameHotUpdate.Battle.Object;

namespace GameHotUpdate.Battle.Event.Turn
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
