
namespace Game.Battle
{
    public class TurnStartEvent : BattleEvent
    {
        /// <summary>
        /// 本回合行动的实体
        /// </summary>
        public IBattleEntityObject CurrentBattleEntity { get; private set; } 

        public TurnStartEvent(IBattleContext context, IBattleEntityObject currentChar) : base(context)
        {
            CurrentBattleEntity = currentChar;
        }
    }
}
