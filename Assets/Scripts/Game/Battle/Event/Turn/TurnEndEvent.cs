
namespace Game.Battle
{
    /// <summary>
    /// 角色行动结束事件
    /// </summary>
    public class TurnEndEvent : BattleEvent
    {
        /// <summary>
        /// 刚结束行动的实体
        /// </summary>
        public IBattleEntityObject CurrentCharacter { get; }

        /// <summary>
        /// 是否在本回合击杀敌人
        /// </summary>
        public bool HasKilledEnemy { get; }

        public TurnEndEvent(IBattleContext context, IBattleEntityObject currentChar, bool hasKilled) : base(context)
        {
            CurrentCharacter = currentChar;
            HasKilledEnemy = hasKilled;
        }
    }
}
