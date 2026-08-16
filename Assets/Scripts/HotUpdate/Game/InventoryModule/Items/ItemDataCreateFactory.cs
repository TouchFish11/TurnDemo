using System;
using Core.DI;
using HotUpdate.Common.Config.Inventory;
using HotUpdate.Common.Config.Inventory.Config;
using HotUpdate.Common.Config.Inventory.Data;

namespace HotUpdate.Game.InventoryModule.Items
{
    /// <summary>
    /// 物品数据创建工厂
    /// </summary>
    public class ItemDataCreateFactory
    {
        /// <summary>
        /// 创建新物品数据
        /// </summary>
        /// <param name="itemConfig"></param>
        /// <param name="originNum"></param>
        /// <param name="persistentId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static ItemData CreateData(ItemConfig itemConfig, int originNum, long? persistentId)
        {
            // 创建数据
            ItemData newItemData = itemConfig.itemType switch
            {
                EItemType.Material => DIContainer.Create<MaterialData>(),
                EItemType.Weapon => DIContainer.Create<WeaponData>(),
                EItemType.HolyRelic => DIContainer.Create<HolyRelicData>(),
                EItemType.precious => DIContainer.Create<preciousData>(),
                _ => throw  new ArgumentOutOfRangeException(nameof(itemConfig.itemType), itemConfig.itemType, null)
            };
            
            // 初始化基础数据
            newItemData.itemId = itemConfig.itemId;
            newItemData.itemNum = originNum;
            newItemData.isNew = true;
            newItemData.persistentId = persistentId;

            // 初始化独特数据
            switch (newItemData)
            {
                case MaterialData materialData:
                    
                    break;
                case WeaponData weaponData:
                    weaponData.level = ((WeaponItemConfig)itemConfig).level;
                    break;
                // TODO:可拓展，在需要生成词条的时候拓展该逻辑
                case HolyRelicData holyRelicData:
                    holyRelicData.level = ((HolyRelicItemConfig)itemConfig).level;
                    break;
                case preciousData preciousData:
                    preciousData.starLevel = ((PreciousItemConfig)itemConfig).starLevel;
                    break;
            }
            
            return newItemData;
        }
    }
}
