using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Statuses
{
    /// <summary>
    /// 战斗状态接口
    /// 定义了战斗中实体所拥有的状态的核心行为和属性
    /// </summary>
    public interface IStatus
    {
        /// <summary>
        /// 状态有效性标识：赋值时自动触发添加/移除逻辑
        /// </summary>
        bool IsValid { get; set; }

        /// <summary>
        /// 状态来源者
        /// 释放该状态的战斗实体
        /// </summary>
        IBattleEntityObject Sourcer { get; }

        /// <summary>
        /// 状态拥有者
        /// 承受该状态的战斗实体
        /// </summary>
        IBattleEntityObject Owner { get; }

        /// <summary>
        /// 状态属性
        /// 存储该状态的基础配置信息
        /// </summary>
        StatusProperty StatusProperty { get; }

        /// <summary>
        /// 状态配置信息
        /// </summary>
        StatusInfo StatusInfo { get; }

        /// <summary>
        /// 状态是否已经生效并参与结算
        /// </summary>
        EStatusState StatusState { get; }

        /// <summary>
        /// 回合开始时的生效逻辑
        /// 在状态拥有者的回合开始时执行
        /// </summary>
        /// <param name="owner">状态拥有者</param>
        /// <param name="context">战斗上下文（包含战斗全局信息、回合信息等）</param>
        void TurnStart(IBattleEntityObject owner, IBattleContext context);

        /// <summary>
        /// 回合结束时的生效逻辑
        /// 在状态拥有者的回合结束时执行
        /// </summary>
        /// <param name="owner">状态拥有者</param>
        /// <param name="context">战斗上下文（包含战斗全局信息、回合信息等）</param>
        void TurnEnd(IBattleEntityObject owner, IBattleContext context);

        /// <summary>
        /// 初始化状态
        /// 状态创建时的初始化方法，用于绑定来源者、拥有者，加载状态配置等
        /// </summary>
        /// <param name="sorucer">状态来源者</param>
        /// <param name="owner">状态拥有者</param>
        /// <param name="statusInfo">状态信息</param>
        void InitStatus(IBattleEntityObject sorucer, IBattleEntityObject owner, StatusInfo statusInfo);

        /// <summary>
        /// 修改状态层数
        /// 用于外部调整状态的叠加层数（如增加buff层数、减少debuff层数）
        /// </summary>
        /// <param name="deltaPine">层数变化值（正数增加层数，负数减少层数）</param>
        void ChangePine(int deltaPine);

        /// <summary>
        /// 启用状态生效并参与结算
        /// </summary>
        void EnableActive();
    }
}