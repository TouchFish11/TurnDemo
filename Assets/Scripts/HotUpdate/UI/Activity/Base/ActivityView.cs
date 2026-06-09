using System.Collections.Generic;
using Core.AssetBundles.Management;
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
        
        // 活动UI选项缓存
        private readonly List<ActivityUI> _actvityUis = new();
        private ActivityUIBehaviourBase _activity;
        
        /// <summary>
        /// 当前显示的活动内容
        /// </summary>
        public IActivity CurrentActivity => _activity;
        
        /// <summary>
        /// 活动选项容器
        /// </summary>
        public RectTransform SvActivityContent => svActivity.content;
        
        /// <summary>
        /// 活动选项ToggleGroup
        /// </summary>
        public ToggleGroup ActivityGroup => ActivityContent;
        
        public void CacheActivityUI(ActivityUI actvityUi)
        {
            _actvityUis.Add(actvityUi);
        }

        public ActivityUI GetFirstActivityUI()
        {
            return _actvityUis.Count > 0 ? _actvityUis[0] : null;
        }

        public void UpdateActivityDetailUI(ActivityUIBehaviourBase actvity, ObjectSpawner spawner)
        {
            spawner.Release(_activity);
            _activity = actvity;
        }
    }
}
