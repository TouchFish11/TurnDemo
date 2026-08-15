using Core.DI;
using Core.Serialize.Binary;
using Core.Utility;
using HotUpdate.Game.Dialogue.Datas;

namespace HotUpdate.Game.Dialogue.Sources
{
    /// <summary>
    /// 对话配置数据源
    /// </summary>
    public class DialogueConfigBranchDataSource : BranchDataSource
    {
        [Inject] private IBinaryDataManager _binaryDataManager;
        
        protected override void AddBranchInfos(DialogueContext dialogueContext)
        {
            var currentDialogueInfo = dialogueContext.CurrentDialogueInfo;
            if (currentDialogueInfo.f_hasBranch)
            {
                // 解析分支ID数组（配置表中以特定格式存储）
                var branchIds = TextUtility.SplitToIntArr(currentDialogueInfo.f_branchIds, 2);
                // 遍历分支ID，从配置表加载分支信息
                foreach (var branchId in branchIds)
                {
                    var branchInfo = _binaryDataManager.GetConfig<BranchInfoContainer>(EConfigLoadType.Excel).dataDic[branchId];
                    // 构造分支数据
                    var branchData = new BranchData(EBranchType.Dialogue, branchInfo);
                    ResultBranches.Add(branchData);
                }
            }
        }
    }
}
