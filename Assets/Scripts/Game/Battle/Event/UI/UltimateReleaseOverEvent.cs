
namespace Game.Battle
{
    /// <summary>
    /// 终结技释放结束事件
    /// 当前角色是玩家时才会处理
    /// 显示当前玩家角色的操作UI
    /// </summary>
    public class UltimateReleaseOverEvent : BattleEvent
    {
        public IBattleEntityObject CurrentActEntity { get; }

        public UltimateReleaseOverEvent(IBattleContext context, IBattleEntityObject currentEntity) : base(context)
        {
            CurrentActEntity = currentEntity;
        }
    }
}
