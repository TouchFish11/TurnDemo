
namespace Game.Battle
{
    /// <summary>
    /// 所有玩家角色死亡条件
    /// </summary>
    public class AllPlayerDeadCondition : IBattleOverCondition
    {
        public bool CheckOver(IBattleContext context)
        {
            return context.GetLivePlayerObjects().Count == 0;
        }
    }
}
