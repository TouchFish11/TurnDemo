using Core.DI;
using Core.Pool;
using HotUpdate.Common.Config.Inventory;

namespace HotUpdate.Game.InventoryModule.Items
{
    /// <summary>
    /// 物品创建工厂
    /// </summary>
    public class ItemCreateFactory : IPoolData
    {
        [Inject] private IPoolManager _poolManager;
        
        /// <summary>
        /// 创建物品对象
        /// </summary>
        /// <returns></returns>
        public Item CreateItem()
        {
            var item = _poolManager.GetData<Item>();
            item.persistentId = 0;
            item.itemConfig = null;
            item.auxValue = 0;
            item.isNew = false;
            item.isDeleted = false;
            return item;
        }

        void IPoolData.ResetData()
        {
            _poolManager = null;
        }
    }
}
