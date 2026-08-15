using System.Collections.Generic;
using Core.DI;
using HotUpdate.Game.Dialogue.Datas;
using HotUpdate.Game.InventoryModule.Items;

namespace HotUpdate.Game.Dialogue.Sources
{
    /// <summary>
    /// 物品提交对话数据源
    /// </summary>
    public class ItemSubmitBranchDataSource : BranchDataSource
    {
        [Inject] private ItemDataProvider _itemDataProvider;
        
        /// <summary>
        /// 物品ID
        /// </summary>
        public int ItemId { get; set; }
        
        /// <summary>
        /// 提交数量
        /// </summary>
        public int Num { get; set; }
        
        protected override void AddBranchInfos(DialogueContext dialogueContext)
        {
            // Example
            var currentInfo = dialogueContext.CurrentDialogueInfo;
            if (_itemDataProvider.TryGetInstanceData(ItemId, out var data) &&
                currentInfo.f_id == -1 &&
                currentInfo.f_speakerId == -1)
            {
                var config = _itemDataProvider.ConfigMap[data.itemId];
                var branchInfo = new BranchInfo
                {
                    f_id = -1,
                    f_optText = $"提交[{config.name}]",
                    f_dialogueId = -1,
                };
                ResultBranches.Add(new ItemBranchData(EBranchType.ItemSubmit, branchInfo, new Dictionary<int, int>()
                {
                    {ItemId, Num}
                }));
            }
        }
    }
}
