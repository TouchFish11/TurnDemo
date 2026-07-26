using System;
using HotUpdate.Common.Config.Inventory.Data;

namespace HotUpdate.Game.InventoryModule.Items
{
    /// <summary>
    /// 物品类型解析器
    /// </summary>
    public static class ItemResolver
    {
        /// <summary>
        /// 解析不同物品数据类型的数值含义
        /// </summary>
        /// <param name="itemData"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int ResolveAux(ItemData itemData)
        {
            return itemData switch
            {
                MaterialData materialData => materialData.itemNum,
                WeaponData weaponData => weaponData.level,
                HolyRelicData holyRelicData => holyRelicData.level,
                preciousData preciousData => preciousData.itemNum,
                _ => throw new ArgumentOutOfRangeException(nameof(itemData), itemData, null)
            };
        }
    }
}
