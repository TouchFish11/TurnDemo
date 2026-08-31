using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Event.Turn
{
    /// <summary>
    /// 回合结束事件
    /// </summary>
    public class TurnEndEvent : BattleEvent
    {
        /// <summary>
        /// 当前持有回合的对象
        /// </summary>
        public IBattleEntityObject CurrentBattleEntity { get; }

        public TurnEndEvent(IBattleContext context, IBattleEntityObject currentChar) : base(context)
        {
            CurrentBattleEntity = currentChar;
        }
    }
}
