
namespace Game.Battle
{
    /// <summary>
    /// 回合开始状态变化事件
    /// 更新角色状态栏UI
    /// </summary>
    public class TurnStartStatusChangedEvent : BattleEvent
    {
        public IBattleEntityObject CurrentBattleEntity { get; }

        public TurnStartStatusChangedEvent(IBattleContext context, IBattleEntityObject currentBattleEntity) : base(context)
        {
            CurrentBattleEntity = currentBattleEntity;
        }
    }
}
