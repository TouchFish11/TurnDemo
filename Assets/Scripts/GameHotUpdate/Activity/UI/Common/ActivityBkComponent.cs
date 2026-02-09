using Core.UI;
using GameHotUpdate.Activity.UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace GameHotUpdate.Activity.UI.Common
{
    /// <summary>
    /// 活动背景UI组件
    /// </summary>
    public class ActivityBkComponent : ActivityUIComponent
    {
        [Inject] private Image imgActivityBackground;

        protected override async void OnInit()
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
