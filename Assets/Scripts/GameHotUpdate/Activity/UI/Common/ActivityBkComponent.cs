using Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameHotUpdate.Activity.UI.Common
{
    /// <summary>
    /// 活动背景UI组件
    /// </summary>
    public class ActivityBkComponent : BaseUIBehaviour
    {
        [Inject] private Image imgActivityBackground;

        /// <summary>
        /// 设置活动背景
        /// </summary>
        /// <param name="bk"></param>
        public void SetBackGround(Sprite bk)
        {
            imgActivityBackground.sprite = bk;
        }
    }
}
