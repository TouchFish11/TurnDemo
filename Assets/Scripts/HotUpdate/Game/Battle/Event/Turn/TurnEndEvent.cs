using HotUpdate.Base;
using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Event.Turn
{
    /// <summary>
    /// 回合结束事件
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
