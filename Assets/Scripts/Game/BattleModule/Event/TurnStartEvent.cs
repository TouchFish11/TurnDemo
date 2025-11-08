
using GameLogic.BattleMoudule.Core;

namespace GameLogic.BattleMoudule.Event
{
    public class TurnStartEvent : BattleEvent
    {
        /// <summary>
        /// 本回合行动的角色
        /// </summary>
        public IBattleEntity CurrentCharacter { get; private set; } 

        public TurnStartEvent(IBattleContext context, IBattleEntity currentChar) : base(context)
        {
            CurrentCharacter = currentChar;
        }
    }
}
