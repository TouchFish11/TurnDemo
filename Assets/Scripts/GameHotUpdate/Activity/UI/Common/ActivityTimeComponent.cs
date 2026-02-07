using Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameHotUpdate.Activity.UI.Common
{
    /// <summary>
    /// 活动时间UI组件
    /// </summary>
    public class ActivityTimeComponent : BaseUIBehaviour
    {
        [Inject] private Image imgBk;
        [Inject] private TextMeshProUGUI txtTime;
        
        /// <summary>
        /// 设置持续时间背景
        /// </summary>
        /// <param name="bk"></param>
        public void SetBackGround(Sprite bk)
        {
            imgBk.sprite = bk;
        }
        
        /// <summary>
        /// 设置持续时间
        /// </summary>
        /// <param name="txtDurationTime"></param>
        public void SetDurationTime(out TextMeshProUGUI txtDurationTime)
        {
            txtDurationTime = txtTime;
        }
    }
}
