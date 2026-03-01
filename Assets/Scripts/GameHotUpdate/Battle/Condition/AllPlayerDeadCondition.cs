using GameHotUpdate.Battle.Context;

namespace GameHotUpdate.Battle.Condition
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
