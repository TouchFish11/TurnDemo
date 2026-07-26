using System;
using HotUpdate.Common.Config.Inventory;
using UnityEngine;

namespace HotUpdate.Game.InventoryModule.Items
{
    /// <summary>
    /// 物品显示格式化器
    /// </summary>
    public static class ItemFormatter
    {
        /// <summary>
        /// 获取物品数据的数量或等级
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string GetItemNumOrLevel(Item item)
        {
            var itemType = item.itemConfig.itemType;
            // 根据物品类型决定显示数量还是等级
            return itemType switch
            {
                EItemType.Material or EItemType.precious => item.auxValue.ToString(),
                EItemType.Weapon or EItemType.HolyRelic => $"+{item.auxValue}",
                _ => throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null)
            };
        }

        /// <summary>
        /// 获取物品背景颜色
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Color GetBkQualityColor(Item item)
        {
            var qualityType = item.itemConfig.itemQuality;
            var originColor = qualityType switch
            {
                EItemQuality.Normal => Color.gray,
                EItemQuality.Rare => Color.blue,
                EItemQuality.Epitome => Color.magenta,
                EItemQuality.Legend => Color.yellow,
                EItemQuality.Immortality => Color.red,
                _ => throw new ArgumentOutOfRangeException(nameof(qualityType), qualityType, null)
            };
            
            return new Color(originColor.r, originColor.g, originColor.b, 0.5f);
        }
    }
}
