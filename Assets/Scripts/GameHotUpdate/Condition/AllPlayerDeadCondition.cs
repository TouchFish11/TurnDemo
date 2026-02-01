using Game.Battle.Condition;
using Game.Battle.Context;
using GameHotUpdate.Battle.Context;

namespace GameHotUpdate.Condition
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
