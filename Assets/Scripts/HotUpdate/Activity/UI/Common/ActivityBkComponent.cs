using Core.UI;
using HotUpdate.Activity.UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Activity.UI.Common
{
    /// <summary>
    /// 活动背景UI组件
    /// </summary>
    public class ActivityBkComponent : ActivityUIComponent
    {
        [Inject] private Image imgActivityBackground;

        protected override void OnInit()
        {
            
        }
        
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
