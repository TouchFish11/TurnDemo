using Game.Battle.Condition;
using Game.Battle.Context;
using GameHotUpdate.Battle.Context;

namespace GameHotUpdate.Condition
{
    /// <summary>
    /// 所有怪物死亡条件
    /// </summary>
    public class AllMonsterDeadCondition : IBattleOverCondition
    {
        public bool CheckOver(IBattleContext context)
        {
            return context.GetAliveMonsterEntityCount() == 0;
        }
    }
}
