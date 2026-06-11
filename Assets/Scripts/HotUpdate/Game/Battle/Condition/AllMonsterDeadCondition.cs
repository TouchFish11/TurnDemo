using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Condition
{
    /// <summary>
    /// 所有怪物死亡结束条件
    /// </summary>
    public class AllMonsterDeadCondition : IWaveOverCondition
    {
        public bool CheckOver(IBattleContext context)
        {
            // 当前怪物是否全部死亡
            return context.GetAliveMonsterEntityCount() == 0;
        }
    }
}
