
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
        public IBattleEntityObject CurrentBattleEntity { get; }

        public TurnEndEvent(IBattleContext context, IBattleEntityObject currentChar) : base(context)
        {
            CurrentBattleEntity = currentChar;
        }
    }
}
