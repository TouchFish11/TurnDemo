using Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Common.Item.UI
{
    /// <summary>
    /// 物品格子
    /// </summary>
    public class ItemGrid : UIBehaviourBase
    {
        [Inject] private Image imgQuality;
        [Inject] private Image imgIcon;
        [Inject] private TextMeshProUGUI txtNum;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="num"></param>
        /// <param name="quality"></param>
        public void Init(Sprite icon, int num, int quality)
        {
            imgQuality.color = GetQualityColor(quality);
            imgIcon.sprite = icon;
            txtNum.text = num.ToString();
        }

        private static Color GetQualityColor(int quality)
        {
            return (EItemQuality)quality switch
            {
                EItemQuality.Normal => Color.gray,
                EItemQuality.Rare => Color.blue,
                EItemQuality.Precious => new Color(0.73f, 0.33f, 0.83f, 1),
                EItemQuality.Legend => new Color(1.0f, 0.75f, 0.27f, 1),
                _ => Color.white
            };
        }
    }
}
