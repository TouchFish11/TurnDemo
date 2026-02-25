using System.Threading.Tasks;
using Core.AssetBundles.Update.Enum;

namespace Core.AssetBundles.Update
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
        /// 执行状态核心逻辑
        /// 包含该更新阶段的主要业务逻辑（异步执行）
        /// </summary>
        /// <returns>异步执行结果，true表示执行完成且无异常，false表示执行异常</returns>
        Task<UpdateResult> Execute();

        /// <summary>
        /// 退出状态
        /// 状态结束时的清理逻辑，如释放临时资源、重置状态标记等
        /// </summary>
        void Exit();
    }
}