using HotUpdate.Game.Dialogue.Datas;

namespace HotUpdate.Game.Dialogue
{
    /// <summary>
    /// 分支处理器，处理不同的分支选择逻辑
    /// </summary>
    public interface IBranchHandler
    {
        /// <summary>
        /// 分支类型
        /// </summary>
        EBranchType BranchType { get; }

        /// <summary>
        /// 执行分支逻辑
        /// </summary>
        /// <param name="branchData"></param>
        void Execute(BranchData branchData);
    }
}
