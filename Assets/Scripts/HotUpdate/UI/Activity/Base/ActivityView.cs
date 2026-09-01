using System.Collections.Generic;
using Core.UI;
using Core.UI.ViewController;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI.Activity.Base
{
    /// <summary>
    /// 活动界面
    /// </summary>
    public class ActivityView : UIView
    {
        [InjectUI] private ScrollRect svActivity;
        [InjectUI] private ToggleGroup ActivityContent;
        [InjectUI] public Button btnClose;
        
        [InjectUI(1)] public RectTransform ActivityDetailArea { get; private set; }
        
        /// <summary>
        /// 活动UI选项缓存
        /// </summary>
        public List<ActivityUI> ActvityUis { get; } = new();
        
        /// <summary>
        /// 当前显示的活动内容
        /// </summary>
        public IActivity CurrentActivity { get; set; }
        
        /// <summary>
        /// 活动选项容器
        /// </summary>
        public RectTransform SvActivityContent => svActivity.content;
        
        /// <summary>
        /// 活动选项ToggleGroup
        /// </summary>
        public ToggleGroup ActivityGroup => ActivityContent;

        public ActivityUI GetFirstActivityUI()
        {
            return ActvityUis.Count > 0 ? ActvityUis[0] : null;
        }
        
        public void SyncCurrentToggle()
        {
            if (CurrentActivity == null) 
                return;
            
            foreach (var ui in ActvityUis)
            {
                ui.SetSelectedWithoutNotify(ui.ActivityId == CurrentActivity.ActivityId);
            }
        }
    }
}
