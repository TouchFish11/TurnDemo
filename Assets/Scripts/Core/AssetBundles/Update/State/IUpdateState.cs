using Core.AssetBundles.Update.Core;

namespace Core.AssetBundles.Update.State
{
    /// <summary>
    /// 资源包更新状态接口
    /// 定义了资源包更新流程中每个状态节点需要实现的核心行为
    /// </summary>
    public interface IUpdateState
    {
        /// <summary>
        /// 当前更新阶段
        /// 标识该状态节点所属的更新流程阶段（由EUpdatePhase枚举定义）
        /// </summary>
        EUpdatePhase UpdatePhase { get; }

        /// <summary>
        /// 进入状态
        /// 状态激活时的初始化逻辑，如资源初始化、参数重置等
        /// </summary>
        void Enter();

        /// <summary>
        /// 退出状态
        /// 状态结束时的清理逻辑，如释放临时资源、重置状态标记等
        /// </summary>
        void Exit();
    }
}