using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Condition
{
    /// <summary>
    /// 所有玩家死亡条件
    /// </summary>
    public class AllPlayerDeadCondition : IBattleOverCondition
    {
        public bool CheckOver(IBattleContext context)
        {
            return context.GetAlivePlayerEntityCount() == 0;
        }
    }
}
