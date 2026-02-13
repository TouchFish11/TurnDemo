using Core.Service;
using Game.Battle.Condition;
using Game.Battle.Context;
using GameHotUpdate.Manager;

namespace GameHotUpdate.Condition
{
    /// <summary>
    /// 所有回合结束条件
    /// </summary>
    public class AllTurnOverCondition : IBattleOverCondition
    {
        public bool CheckOver(IBattleContext context)
        {
            return ServiceLocator.Get<IBattleManager>().GetTurnCreator().CheckBattleOver();
        }
    }
}
