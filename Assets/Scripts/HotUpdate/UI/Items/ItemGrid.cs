using System;
using Core.UI;
using HotUpdate.Common.Config.Inventory;
using HotUpdate.Common.Config.Inventory.Config;
using HotUpdate.Game.Slot;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HotUpdate.UI.Items
{
    /// <summary>
    /// 奖励物品格子
    /// </summary>
    public class ItemGrid : UIBehaviourBase, IGridInteractive<ItemConfig>, IPointerClickHandler
    {
        [InjectUI] private Image imgQuality;
        [InjectUI] private Image imgIcon;
        [InjectUI] private TextMeshProUGUI txtNum;

        public event Action<ItemConfig> OnSelected;
        
        private ItemConfig _itemConfig;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="num"></param>
        /// <param name="config"></param>
        public void Init(Sprite icon, int num, ItemConfig config)
        {
            imgQuality.color = GetQualityColor(config.itemQuality);
            imgIcon.sprite = icon;
            txtNum.text = num.ToString();
            _itemConfig = config;
        }
        
        private static Color GetQualityColor(EItemQuality quality)
        {
            return quality switch
            {
                EItemQuality.Normal => Color.gray,
                EItemQuality.Rare => Color.blue,
                EItemQuality.Epitome => new Color(0.73f, 0.33f, 0.83f, 1),
                EItemQuality.Legend => new Color(1.0f, 0.75f, 0.27f, 1),
                EItemQuality.Immortality => Color.red,
                _ => Color.white
            };
        }

        protected override void OnDisable()
        {
            _itemConfig = null;
            OnSelected = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnSelected?.Invoke(_itemConfig);
        }
    }
}
