using GameLogic.BattleMoudule.Core;

namespace GameLogic.BattleMoudule.Status
{
    /// <summary>
    /// 状态接口（定义状态的核心行为，高内聚）
    /// </summary>
    public interface IStatus
    {
        /// <summary>
        /// 状态是否有效
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// 回合开始时的生效逻辑
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="context"></param>
        void OnTurnStart(IBattleEntity owner, IBattleContext context);

        /// <summary>
        /// 回合结束时的生效逻辑
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="context"></param>
        void OnTurnEnd(IBattleEntity owner, IBattleContext context); 
    }
}
