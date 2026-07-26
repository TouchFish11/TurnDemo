using System;
using HotUpdate.Common.Config.Inventory;

namespace HotUpdate.Game.InventoryModule.Sorts
{
    /// <summary>
    /// 默认ID排序器
    /// </summary>
    public class DefaultIDSorter : InventorySorter
    {
        protected override Comparison<Item> GetSorter(int i)
        {
            // 默认降序
            return i switch
            {
                0 => null,
                > 0 => (x, y) => x.itemConfig.itemId.CompareTo(y.itemConfig.itemId),    // 升序
                _ => (x, y) => y.itemConfig.itemId.CompareTo(x.itemConfig.itemId)       // 降序
            };
        }
    }
}
