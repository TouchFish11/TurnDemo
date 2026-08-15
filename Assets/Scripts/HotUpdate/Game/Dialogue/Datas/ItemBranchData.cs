using System.Collections.Generic;

namespace HotUpdate.Game.Dialogue.Datas
{
    /// <summary>
    /// 物品分支数据
    /// </summary>
    public class ItemBranchData : BranchData
    {
        public Dictionary<int, int> Items { get; }
        
        /// <summary>
        /// 物品分支数据构造
        /// </summary>
        /// <param name="branchType"></param>
        /// <param name="branchInfo"></param>
        /// <param name="items">key为物品ID，value为数量</param>
        public ItemBranchData(EBranchType branchType, BranchInfo branchInfo, Dictionary<int, int> items) : base(branchType, branchInfo)
        {
            Items = items;
        }
    }
}
