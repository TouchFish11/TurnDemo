using HotUpdate.Common.Config.Inventory.Config;
using HotUpdate.Common.Config.Inventory.Data;
using UnityEngine;

namespace HotUpdate.UI.Inventory
{
    public interface IInventoryDetailPanel
    {
         GameObject DetailPanel { get; }
        
        void UpdateInfo(ItemConfig itemConfig, ItemData itemData);
    }
}
