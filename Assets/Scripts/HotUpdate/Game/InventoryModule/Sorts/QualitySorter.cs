using System;
using HotUpdate.Common.Config.Inventory;

namespace HotUpdate.Game.InventoryModule.Sorts
{
    /// <summary>
    /// 品质类型排序器
    /// </summary>G
    public class QualitySorter : InventorySorter
    {
        protected override Comparison<Item> GetSorter(int i)
        {
            return i switch
            {
                0 => null,
                > 0 => (x, y) => x.itemConfig.itemQuality.CompareTo(y.itemConfig.itemQuality),    // 升序
                _ => (x, y) => y.itemConfig.itemQuality.CompareTo(x.itemConfig.itemQuality)       // 降序
            };
        }
    }
}
