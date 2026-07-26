using System.Collections.Generic;
using Core.UI;
using Core.UI.ViewController;
using HotUpdate.UI.Inventory.ViewModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI.Inventory
{
    /// <summary>
    /// 背包界面
    /// </summary>
    public class InventoryPanel : UIView
    {
        [InjectUI] public ScrollRect svOpts;
        [InjectUI] public ScrollRect svItems;
        [InjectUI] public Dropdown dpSorts;
        [InjectUI] public Button btnClose;
        [InjectUI] public Button btnRequestDelete;
        [InjectUI] public Button btnDelete;
        [InjectUI] public Button btnCancelDelete;
        [InjectUI] public TMP_InputField inputFieldDeleteNum;
        [InjectUI] public Button btnSub;
        [InjectUI] public Button btnAdd;
        [InjectUI] public Button btnMin;
        [InjectUI] public Button btnMax;
        [InjectUI] public Slider sliderNum;
        
        [InjectUI(1)] public RectTransform deleteBox;
        
        [InjectUI(1)] public RectTransform DetailArea { get; private set; }
        
        [InjectUI(1)] public RectTransform DeleteArea { get; private set; }
        
        public ToggleGroup OptGroup {get; private set;}
        
        /// <summary>
        /// 物品类型选项
        /// </summary>
        public List<ItemTypeOpt> ItemTypeOpts { get; private set; } = new();
        
        /// <summary>
        /// 详细界面池化对象
        /// </summary>
        public InventoryDetailPanel DetailPanel { get; set; }
        
        protected override void Awake()
        {
            base.Awake();
            OptGroup = svOpts.content.GetComponent<ToggleGroup>();
            DeleteArea.gameObject.SetActive(false);
        }

        public void SetViewModel(InventoryDeleteViewModel inventoryDeleteViewModel)
        {
            inventoryDeleteViewModel.DeleteAreaActive.Subscribe(SetDeleteAreaActive);
            inventoryDeleteViewModel.DeleteBoxActive.Subscribe(SetDeleteBoxActive);
            inventoryDeleteViewModel.DeleteSliderExtremum.Subscribe(SetDeleteSliderExtremum);
            inventoryDeleteViewModel.DeleteNum.Subscribe(SetDeleteSliderNum);
            inventoryDeleteViewModel.DeleteNum.Subscribe(SetInputFieldDeleteNum);
            
            inventoryDeleteViewModel.AddDeleteBtnEnable.Subscribe(SetAddDeleteBtnEnable);
            inventoryDeleteViewModel.SubDeleteBtnEnable.Subscribe(SetSubDeleteBtnEnable);
            inventoryDeleteViewModel.MaxDeleteBtnEnable.Subscribe(SetMaxDeleteBtnEnable);
            inventoryDeleteViewModel.MinDeleteBtnEnable.Subscribe(SetMinDeleteBtnEnable);
        }

        private void SetDeleteAreaActive(bool isActive)
        {
            DeleteArea.gameObject.SetActive(isActive);
        }
        
        /// <summary>
        /// 设置删除盒显隐状态
        /// </summary>
        /// <param name="isActive"></param>
        private void SetDeleteBoxActive(bool isActive)
        {
            deleteBox.gameObject.SetActive(isActive);
        }

        private void SetDeleteSliderExtremum((float min, float max) extremum)
        {
            sliderNum.minValue = extremum.min;
            sliderNum.maxValue = extremum.max;
        }
        
        private void SetDeleteSliderNum(int deleteNum)
        {
            sliderNum.SetValueWithoutNotify(deleteNum);
        }
        
        private void SetInputFieldDeleteNum(int deleteNum)
        {
            inputFieldDeleteNum.SetTextWithoutNotify(deleteNum.ToString());
        }

        private void SetAddDeleteBtnEnable(bool enable)
        {
            btnAdd.enabled = enable;
        }

        private void SetSubDeleteBtnEnable(bool enable)
        {
            btnSub.enabled = enable;
        }

        private void SetMaxDeleteBtnEnable(bool enable)
        {
            btnMax.enabled = enable;
        }

        private void SetMinDeleteBtnEnable(bool enable)
        {
            btnMin.enabled = enable;
        }

        /// <summary>
        /// 获取第一个类型的选项
        /// </summary>
        /// <returns></returns>
        public ItemTypeOpt GetFirstItemTypeOpt()
        {
            return ItemTypeOpts[0];
        }

        /// <summary>
        /// 缓存类型选项UI
        /// </summary>
        /// <param name="itemTypeOpt"></param>
        public void AddItemTypeOpt(ItemTypeOpt itemTypeOpt)
        {
            ItemTypeOpts.Add(itemTypeOpt);   
        }
    }
}
