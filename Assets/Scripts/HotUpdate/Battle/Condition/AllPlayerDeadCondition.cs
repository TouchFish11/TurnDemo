using HotUpdate.Battle.Context;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Condition;

namespace HotUpdate.Battle.Condition
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
