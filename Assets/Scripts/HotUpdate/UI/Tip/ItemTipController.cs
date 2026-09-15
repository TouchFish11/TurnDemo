using System.Threading.Tasks;
using Core.DI;
using Core.UI.ViewController;
using HotUpdate.Base.Service;
using HotUpdate.Base.UI;
using HotUpdate.Common.Config.Inventory.Config;
using HotUpdate.Game.InventoryModule.Items;

namespace HotUpdate.UI.Tip
{
    public class ItemTipController : UIController<ItemTipView>
    {
        [Inject] private IUIService _uiservice;
        [Inject] private ItemDataProvider _itemDataProvider;
        [Inject] private IIconService  _iconService;

        private ItemConfig _itemConfig;
        
        protected override Task OnInit()
        {
            return Task.CompletedTask;
        }

        protected override Task OnActive()
        {
            return Task.CompletedTask;
        }

        public async Task Init(ItemConfig itemConfig)
        {
            view.imgIcon.sprite = await _iconService.LoadIconAsync(itemConfig.icon);
            view.txtItemName.text = itemConfig.name;
            view.txtDescription.text = itemConfig.description;
            view.txtHoldNumInfo.text = _itemDataProvider.TryGetData(itemConfig.itemId, out var itemData) ? itemData.itemNum.ToString() : 0.ToString();
            _itemConfig = itemConfig;
        }

        protected override void OnButtonClick(string btnName)
        {
            if (btnName is nameof(view.btnBoxClose) or nameof(view.btnClose))
            {
                _uiservice.CloseAsync(panelId, false);
            }
        }

        protected override Task OnInactivate()
        {
            return Task.CompletedTask;
        }
    }
}
