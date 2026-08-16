using System.Collections.Generic;
using System.Text;
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
        
        public List<(int itemId, int num, long? persistentId)> SubmitItems { get; } = new();

        /// <summary>
        /// 是否还有对话，有则为指定对话ID，否则为-1
        /// </summary>
        /// <value>
        /// 默认-1
        /// </value>
        public int NextDialogueId { get; set; } = -1;
        
        protected override void AddBranchInfos(DialogueContext dialogueContext)
        {
            var currentInfo = dialogueContext.CurrentDialogueInfo;
            var tempSubmit = new List<(int itemId, int num, long? persistentId)>();
            foreach (var (itemId, num, persistentId) in SubmitItems)
            {
                if (_itemDataProvider.TryGetData(itemId, out var data, persistentId) &&
                    currentInfo.f_id == -1 &&
                    currentInfo.f_speakerId == -1)
                {
                    tempSubmit.Add((itemId, num, persistentId));
                }
            }

            var sb = new StringBuilder(32);
            sb.Append("提交 ");
            foreach (var (itemId, num, _) in tempSubmit)
            {
                var config = _itemDataProvider.ConfigMap[itemId];
                sb.Append($"[{config.name}] x{num}, ");
            }
            
            var branchInfo = new BranchInfo
            {
                f_id = -1,
                f_optText = sb.ToString(),
                f_dialogueId = NextDialogueId
            };
            ResultBranches.Add(new ItemBranchData(EBranchType.ItemSubmit, branchInfo, tempSubmit));
        }
    }
}
