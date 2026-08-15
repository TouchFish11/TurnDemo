using System.Collections.Generic;
using Core.Pool;
using HotUpdate.Game.Dialogue.Datas;

namespace HotUpdate.Game.Dialogue.Sources
{
    /// <summary>
    /// 对话分支抽象来源类
    /// </summary>
    public abstract class BranchDataSource : IBranchDataSource
    {
        /// <summary>
        /// 返回的对话分支信息列表
        /// </summary>
        protected List<BranchData> ResultBranches { get; } = new();
        
        public IEnumerable<BranchData> GetBranchDatas(DialogueContext dialogueContext)
        {
            ResultBranches.Clear();
            AddBranchInfos(dialogueContext);
            return ResultBranches;
        }

        /// <summary>
        /// 添加分支到<see cref="ResultBranches"/>逻辑
        /// </summary>
        /// <param name="dialogueContext"></param>
        protected abstract void AddBranchInfos(DialogueContext dialogueContext);
    
        void IPoolData.ResetData()
        {
            ResultBranches.Clear();
        }
    }
}
