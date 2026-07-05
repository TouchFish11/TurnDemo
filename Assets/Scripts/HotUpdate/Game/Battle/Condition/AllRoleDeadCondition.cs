using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Condition
{
    /// <summary>
    /// 所有玩家角色死亡条件
    /// </summary>
    public class AllRoleDeadCondition : IWaveOverCondition
    {
        public bool CheckOver(IBattleContext context)
        {
            // 当前角色是否全部死亡
            return context.GetAlivePlayerEntityCount() == 0;
        }
    }
}
