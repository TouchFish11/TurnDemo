using Core.UI;
using HotUpdate.Common.Config.Inventory.Config;
using HotUpdate.Common.Config.Inventory.Data;
using UnityEngine;

namespace HotUpdate.UI.Inventory
{
    public abstract class InventoryDetailPanel : UIBehaviourBase, IInventoryDetailPanel
    {
        protected ItemConfig itemConfig;
        protected ItemData itemData;
        
        public GameObject DetailPanel => this.gameObject;
    
        public void UpdateInfo(ItemConfig itemConfig, ItemData itemData)
        {
            this.itemConfig = itemConfig;
            this.itemData = itemData;
            OnUpdateInfo();
        }
        
        protected abstract void OnUpdateInfo();
    }
}
