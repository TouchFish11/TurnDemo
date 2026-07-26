using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Pool;
using HotUpdate.Common.Config.Inventory;
using HotUpdate.UI.Inventory.Detail;
using UnityEngine;

namespace HotUpdate.UI.Inventory
{
    /// <summary>
    /// 背包详细界面创建工厂
    /// </summary>
    public class InventoryDetailViewCreateFactory : IPoolData
    {
        [Inject] private ObjectSpawner _objectSpawner;
        
        public async Task<IInventoryDetailPanel> CreateDetailPanel(EItemType itemType, RectTransform detailArea)
        {
            switch (itemType)
            {
                case EItemType.Material:
                    var materialDetailPanel = await _objectSpawner.SpawnAsync<MaterialDetailPanel>(AssetKeys.MaterialDetailPanel, detailArea, Vector2.zero);
                    return materialDetailPanel;
                case EItemType.Weapon:
                    var weaponDetailPanel = await _objectSpawner.SpawnAsync<WeaponDetailPanel>(AssetKeys.WeaponDetailPanel, detailArea, Vector2.zero);
                    return weaponDetailPanel;
                case EItemType.HolyRelic:
                    // TODO：await _objectSpawner.SpawnAsync<MaterialDetailPanel>("HolyRelic", detailArea);
                    return null;
                case EItemType.precious:
                    // TODO：await _objectSpawner.SpawnAsync<MaterialDetailPanel>("precious", detailArea);
                    return null;
                default:
                    return null;
            }
        }
        
        void IPoolData.ResetData()
        {
            _objectSpawner.Dispose();
            _objectSpawner = null;
        }
    }
}
