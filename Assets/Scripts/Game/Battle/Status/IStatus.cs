
namespace Game.Battle
{
    /// <summary>
    /// 状态接口
    /// </summary>
    public interface IStatus
    {
        /// <summary>
        /// 状态是否有效
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// 来源
        /// </summary>
        IBattleEntityObject Sourcer { get; }

        /// <summary>
        /// 拥有
        /// </summary>
        IBattleEntityObject Owner { get; }

        /// <summary>
        /// 状态信息
        /// </summary>
        StatusInfo StatusInfo { get; }

        /// <summary>
        /// 回合开始时的生效逻辑
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="context"></param>
        void OnTurnStart(IBattleEntityObject owner, IBattleContext context);

        /// <summary>
        /// 回合结束时的生效逻辑
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="context"></param>
        void OnTurnEnd(IBattleEntityObject owner, IBattleContext context);

        /// <summary>
        /// 初始化状态
        /// </summary>
        /// <param name="sorucer"></param>
        /// <param name="owner"></param>
        /// <param name="statusInfo"></param>
        void InitStatus(IBattleEntityObject sorucer, IBattleEntityObject owner, StatusInfo statusInfo);
    }
}
