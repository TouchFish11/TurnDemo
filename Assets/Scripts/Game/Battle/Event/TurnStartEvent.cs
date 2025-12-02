
namespace Game.Battle
{
    public class TurnStartEvent : BattleEvent
    {
        /// <summary>
        /// 本回合行动的角色
        /// </summary>
        public IBattleEntityObject CurrentBattleObject { get; private set; } 

        public TurnStartEvent(IBattleContext context, IBattleEntityObject currentChar) : base(context)
        {
            CurrentBattleObject = currentChar;
        }
    }
}
