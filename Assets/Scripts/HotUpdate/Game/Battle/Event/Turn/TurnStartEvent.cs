using HotUpdate.Base;
using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Event.Turn
{
    public class TurnStartEvent : BattleEvent
    {
        /// <summary>
        /// 回合开始事件
        /// </summary>
        public IBattleEntityObject CurrentBattleEntity { get; private set; } 

        public TurnStartEvent(IBattleContext context, IBattleEntityObject currentChar) : base(context)
        {
            CurrentBattleEntity = currentChar;
        }
    }
}
