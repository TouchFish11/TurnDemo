using System.Collections;

namespace Framework
{
    /// <summary>
    /// 更新状态接口
    /// </summary>
    public interface IUpdateState
    {
        /// <summary>
        /// 更新阶段
        /// </summary>
        E_UpdatePhase UpdatePhase { get; }

        /// <summary>
        /// 当前阶段是否成功完成
        /// </summary>
        bool IsSuceess { get; set; }

        /// <summary>
        /// 进入状态
        /// </summary>
        void Enter();

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        IEnumerator Execute();

        /// <summary>
        /// 退出状态
        /// </summary>
        void Exit();
    }
}
