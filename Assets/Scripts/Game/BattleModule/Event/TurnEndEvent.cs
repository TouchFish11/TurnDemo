using GameLogic.BattleMoudule.Core;

namespace GameLogic.BattleMoudule.Event
{
    /// <summary>
    /// 角色行动结束事
    /// </summary>
    public class TurnEndEvent : BattleEvent
    {
        /// <summary>
        /// 刚结束行动的实体
        /// </summary>
        public IBattleEntity CurrentCharacter { get; }

        /// <summary>
        /// 是否在本回合击杀敌人
        /// </summary>
        public bool HasKilledEnemy { get; }

        public TurnEndEvent(IBattleContext context, IBattleEntity currentChar, bool hasKilled) : base(context)
        {
            CurrentCharacter = currentChar;
            HasKilledEnemy = hasKilled;
        }
    }
}
