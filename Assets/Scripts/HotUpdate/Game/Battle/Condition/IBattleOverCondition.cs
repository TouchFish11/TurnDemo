using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Condition
{
    /// <summary>
    /// 战斗结束条件接口
    /// </summary>
    public interface IBattleOverCondition
    {
        /// <summary>
        /// 检查战斗是否结束，true为结束
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool CheckOver(IBattleContext context);
    }
}
