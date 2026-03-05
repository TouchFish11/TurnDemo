using Core.UI;
using Core.UI.MVC;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Activity.UI.Base
{
    public class ActivityView : UIView
    {
        [Inject] private ScrollRect svActivity;
        [Inject] private ToggleGroup ActivityContent;
        [Inject] public Button btnClose;
        
        [Inject(1)] public RectTransform ActivityDetailArea { get; private set; }
        
        /// <summary>
        /// 活动选项容器
        /// </summary>
        public RectTransform SvActivityContent => svActivity.content;
        
        /// <summary>
        /// 活动选项ToggleGroup
        /// </summary>
        public ToggleGroup ActivityGroup => ActivityContent;
    }
}
