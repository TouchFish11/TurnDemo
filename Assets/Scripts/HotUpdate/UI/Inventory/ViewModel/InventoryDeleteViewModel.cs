using System;
using Core.UI;

namespace HotUpdate.UI.Inventory.ViewModel
{
    /// <summary>
    /// 背包删除物品VM
    /// </summary>
    public class InventoryDeleteViewModel : IDisposable
    {
        public ReactiveProperty<bool> DeleteAreaActive { get; private set; } = new();
        
        public ReactiveProperty<bool> DeleteBoxActive { get; private set; } = new();

        public ReactiveProperty<(float min, float max)> DeleteSliderExtremum { get; private set; } = new();
        
        public ReactiveProperty<bool> AddDeleteBtnEnable { get; private set; } = new();
        
        public ReactiveProperty<bool> SubDeleteBtnEnable { get; private set; } = new();

        public ReactiveProperty<bool> MinDeleteBtnEnable { get; private set; } = new();
        
        public ReactiveProperty<bool> MaxDeleteBtnEnable { get; private set; } = new();
        
        public ReactiveProperty<int> DeleteNum { get; private set; } = new();

        /// <summary>
        /// 重置属性值数据，避免上次残留
        /// </summary>
        public void ResetData()
        {
            DeleteAreaActive.Value = false;
            DeleteBoxActive.Value = false;
            DeleteSliderExtremum.Value = default;
            AddDeleteBtnEnable.Value = false;
            SubDeleteBtnEnable.Value = false;
            MinDeleteBtnEnable.Value = false;
            MaxDeleteBtnEnable.Value = false;
            DeleteNum.Value = 0;
        }
        
        /// <summary>
        /// 销毁VM
        /// </summary>
        public void Dispose()
        {
            DeleteAreaActive.Dispose();
            DeleteBoxActive.Dispose();
            DeleteSliderExtremum.Dispose();
            DeleteNum.Dispose();
            AddDeleteBtnEnable.Dispose();
            SubDeleteBtnEnable.Dispose();
            MinDeleteBtnEnable.Dispose();
            MaxDeleteBtnEnable.Dispose();

            DeleteAreaActive = null;
            DeleteBoxActive = null;
            DeleteSliderExtremum = null;
            AddDeleteBtnEnable = null;
            SubDeleteBtnEnable = null;
            MinDeleteBtnEnable = null;
            MaxDeleteBtnEnable = null;
            DeleteNum = null;
        }
    }
}
