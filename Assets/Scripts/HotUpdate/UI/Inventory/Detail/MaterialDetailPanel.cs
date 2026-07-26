using Core.UI;
using TMPro;

namespace HotUpdate.UI.Inventory.Detail
{
    /// <summary>
    /// 材料类型详细界面
    /// </summary>
    public class MaterialDetailPanel : InventoryDetailPanel
    {
        [InjectUI] private TextMeshProUGUI txtMaterialName;
        [InjectUI] private TextMeshProUGUI txtMaterialDescription;
        
        protected override void OnUpdateInfo()
        {
            txtMaterialName.text = itemConfig.name;
            txtMaterialDescription.text = itemConfig.description;
        }
    }
}
