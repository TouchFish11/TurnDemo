using Core.DI;
using HotUpdate.Game.Dialogue.Datas;
using HotUpdate.Game.InventoryModule.Items;

namespace HotUpdate.Game.Dialogue.Handler
{
    /// <summary>
    /// 提交物品分支处理器
    /// </summary>
    public class ItemSubmitBranchHandler : IBranchHandler
    {
        [Inject] private ItemDataProvider _itemDataProvider;
        
        public EBranchType BranchType => EBranchType.ItemSubmit;
    
        public void Execute(BranchData branchData)
        {
            var itemBranchData = (ItemBranchData)branchData;
            foreach (var (id, num, persistentId) in itemBranchData.Items)
            {
                _itemDataProvider.RemoveData(id, num, persistentId);
            }
        }
    }
}
