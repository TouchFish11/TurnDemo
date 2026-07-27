using System;
using Core.UI;
using HotUpdate.Base.Service;
using HotUpdate.Common.Config.Inventory;
using HotUpdate.Game.InventoryModule.Items;
using HotUpdate.Game.Slot;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HotUpdate.UI.Inventory
{
    /// <summary>
    /// 物品格子
    /// </summary>
    public class ItemCell : UIBehaviourBase, IGridBase<Item>
    {
        [InjectUI] private Image imgBkQuality;
        [InjectUI] private Image imgIcon;
        [InjectUI] private TextMeshProUGUI txtNumOrLv;
        [InjectUI] private Button btnCell;
        [InjectUI] private Image imgHighlight;
        [InjectUI] private Image imgDeleteFlag;
        
        // 物品对象
        private Item _item;
        
        [InjectUI(1)] private RectTransform New { get; set; }
        
        public bool Selected { get; set; }
        
        /// <summary>
        /// 物品点击事件
        /// </summary>
        private Action<Item> _onClick;

        protected override void OnEnable()
        {
            // 默认隐藏
            imgHighlight.gameObject.SetActive(false);
            imgDeleteFlag.gameObject.SetActive(false);
            New.gameObject.SetActive(false);
        }

        /// <summary>
        /// 初始化物品格子
        /// </summary>
        /// <param name="item"></param>
        /// <param name="iconService"></param>
        public void InitGrid(Item item, IIconService iconService)
        {
            imgBkQuality.color = ItemFormatter.GetBkQualityColor(item);
            imgIcon.sprite = iconService.TryGetIcon(item.itemConfig.icon, out var icon) ? icon : null;
            // 根据物品的类型返回不同的数值格式化内容
            txtNumOrLv.text = ItemFormatter.GetItemNumOrLevel(item);
            _item = item;
            // 更新显示状态
            UpdateState();
        }
        
        public void SetClick(Action<Item> OnClick)
        {
            _onClick = OnClick;
        }

        private void UpdateState()
        {
            // 是否是新物品
            New.gameObject.SetActive(_item.isNew);
            // 是否是待删除物品
            imgDeleteFlag.gameObject.SetActive(_item.isDeleted);
        }

        /// <summary>
        /// 选中当前物品格子
        /// </summary>
        public void TriggerClick()
        {
            _onClick?.Invoke(_item);
            // 隐藏New标志
            New.gameObject.SetActive(false);
            // 切换删除标志显示/隐藏
            imgDeleteFlag.gameObject.SetActive(_item.isDeleted);
        }

        /// <summary>
        /// 清理格子的删除状态，隐藏删除标记显示
        /// </summary>
        public void ClearDeleteState()
        {
            // 清理删除标志
            imgDeleteFlag.gameObject.SetActive(false);
        }

        protected override void OnButtonClick(string btnName)
        {
            if (btnName == nameof(btnCell))
            {
                TriggerClick();
            }
        }

        protected override void OnDisable()
        {
            _onClick = null;
            _item = null;
        }
        
        protected override void OnPointerEnter(PointerEventData eventData)
        {
            imgHighlight?.gameObject.SetActive(true);
        }

        protected override void OnPointerExit(PointerEventData eventData)
        {
            imgHighlight?.gameObject.SetActive(false);
        }
    }
}
