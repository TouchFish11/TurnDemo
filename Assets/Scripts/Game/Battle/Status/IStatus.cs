
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
        bool IsValid { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        IBattleEntityObject Sourcer { get; }

        /// <summary>
        /// 拥有
        /// </summary>
        IBattleEntityObject Owner { get; }

        /// <summary>
        /// 状态属性
        /// </summary>
        StatusProperty StatusProperty { get; }

        /// <summary>
        /// 状态加成数据
        /// </summary>
        StatusBonusData BonusData { get; }

        /// <summary>
        /// 回合开始时的生效逻辑
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="context"></param>
        void TurnStart(IBattleEntityObject owner, IBattleContext context);

        /// <summary>
        /// 回合结束时的生效逻辑
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="context"></param>
        void TurnEnd(IBattleEntityObject owner, IBattleContext context);

        /// <summary>
        /// 初始化状态
        /// </summary>
        /// <param name="sorucer"></param>
        /// <param name="owner"></param>
        /// <param name="statusId"></param>
        void InitStatus(IBattleEntityObject sorucer, IBattleEntityObject owner, int statusId);

        /// <summary>
        /// 改变层数
        /// 用于外部修改状态层数
        /// </summary>
        /// <param name="deltaPine"></param>
        void ChangePine(int deltaPine);
    }
}
