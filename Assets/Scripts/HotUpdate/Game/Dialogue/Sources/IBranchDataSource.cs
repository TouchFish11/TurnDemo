using System.Collections.Generic;
using Core.Pool;
using HotUpdate.Game.Dialogue.Datas;

namespace HotUpdate.Game.Dialogue.Sources
{
    /// <summary>
    /// 对话分支来源类，抽象分支的数据来源
    /// </summary>
    public interface IBranchDataSource : IPoolData
    {
        /// <summary>
        /// 获取分支信息
        /// </summary>
        /// <param name="dialogueContext"></param>
        /// <returns></returns>
        IEnumerable<BranchData> GetBranchDatas(DialogueContext dialogueContext);
    }
}
