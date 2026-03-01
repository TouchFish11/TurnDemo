using Core.Service;
using GameHotUpdate.Battle.Context;
using GameHotUpdate.Battle.Core;

namespace GameHotUpdate.Battle.Condition
{
    /// <summary>
    /// 所有回合结束条件
    /// </summary>
    public class AllTurnOverCondition : IBattleOverCondition
    {
        public bool CheckOver(IBattleContext context)
        {
            // 当前波次是否结束，即判断当前怪物是否全部死亡
            if (context.GetAliveMonsterEntityCount() != 0)
            {
                return false;
            }
            
            return ServiceLocator.Get<IBattleManager>().GetTurnCreator().CheckBattleOver();
        }
    }
}
