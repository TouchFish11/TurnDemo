using System.Collections.Generic;
using System.Threading.Tasks;
using HotUpdate.Common.Config.Inventory;
using HotUpdate.Common.Config.Inventory.Data;

namespace HotUpdate.Game.InventoryModule.Items
{
    /// <summary>
    /// 背包管理器接口
    /// </summary>
    public interface IInventoryManager
    {
        /// <summary>
        /// 异步创建物品对象
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        Task<List<Item>> CreateItemsAsync(EItemType itemType);

        /// <summary>
        /// 通过物品实例ID获取物品数据
        /// </summary>
        /// <param name="item">物品对象</param>
        /// <returns></returns>
        ItemData GetData(Item item);

        /// <summary>
        /// 清理当前界面管理的数据
        /// </summary>
        void Clear();

        /// <summary>
        /// 更新格子数据的New状态
        /// </summary>
        /// <param name="item">物品对象</param>
        void UpdateGridNewState(Item item);

        /// <summary>
        /// 删除可堆叠的物品
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="deleteNum"></param>
        void DeleteItem(int itemId, int deleteNum);
        
        /// <summary>
        /// 删除不可堆叠物品
        /// </summary>
        /// <param name="persistentId"></param>
        void DeleteItem(long persistentId);
        
        /// <summary>
        /// 获取当前类型显示的所有物品对象，只能获取所有可堆叠/不可堆叠的物品对象
        /// 当同一类型存在可堆叠和不可堆叠的物品时，无法准确获取
        /// </summary>
        /// <returns></returns>
        IEnumerable<Item> GetAllItems();
    }
}
