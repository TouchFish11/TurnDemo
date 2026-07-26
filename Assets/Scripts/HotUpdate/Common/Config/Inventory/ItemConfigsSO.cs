using System;
using System.Collections.Generic;
using Core.SO;
using HotUpdate.Common.Config.Inventory.Config;
using UnityEngine;

namespace HotUpdate.Common.Config.Inventory
{
    /// <summary>
    /// 所有物品配置SO
    /// </summary>
    [CreateAssetMenu(fileName = nameof(ItemConfigsSO), menuName = "SO/ItemConfigsSO")]
    public class ItemConfigsSO : SOBase
    {
        private static readonly Dictionary<EItemType, Type> TypeMap = new()
        {
            { EItemType.Material , typeof(MaterialItemConfig)},
            { EItemType.precious, typeof(PreciousItemConfig) },
            { EItemType.Weapon, typeof(WeaponItemConfig) },
            { EItemType.HolyRelic, typeof(HolyRelicItemConfig) },
        };
        
        // 物品配置集合
        [SerializeReference] public ItemConfigCollection itemConfigCollection;

        protected override void OnAwake()
        {
            if (itemConfigCollection != null && itemConfigCollection.itemConfigs != null) 
                return;
            
            itemConfigCollection = new ItemConfigCollection
            {
                itemConfigs = new List<ItemConfig>()
            };
                
            Debug.Log($"[{nameof(ItemConfigsSO)}]: 已被初始化");
        }

        private void OnValidate()
        {
            var list = itemConfigCollection.itemConfigs;
            
            for (int i = 0; i < list.Count; i++)
            {
                var current = list[i];

                if (current == null)
                {
                    list[i] = new ItemConfig();
                    continue;
                }

                if (!TypeMap.TryGetValue(current.itemType, out var targetType))
                {
                    targetType = typeof(ItemConfig);
                }

                if (current.GetType() != targetType)
                {
                    list[i] = (ItemConfig)Activator.CreateInstance(targetType);
                    list[i].itemType = current.itemType;
                }
            }
            target = itemConfigCollection;
        }
    }
}
