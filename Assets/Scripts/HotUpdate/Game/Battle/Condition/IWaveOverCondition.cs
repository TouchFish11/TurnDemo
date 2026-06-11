using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Condition
{
    /// <summary>
    /// 波次结束条件接口
    /// </summary>
    public interface IWaveOverCondition
    {
        /// <summary>
        /// 检查波次是否结束，true为结束
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool CheckOver(IBattleContext context);
    }
}
