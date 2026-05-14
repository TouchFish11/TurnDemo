using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.UI;
using Core.UI.ViewController;
using HotUpdate.Activity.Core;
using HotUpdate.Base.Activity;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Activity.UI.Base
{
    public class ActivityView : UIView
    {
        [InjectUI] private ScrollRect svActivity;
        [InjectUI] private ToggleGroup ActivityContent;
        [InjectUI] public Button btnClose;
        
        [InjectUI(1)] public RectTransform ActivityDetailArea { get; private set; }
        
        private readonly List<PoolObject> _actvityUis = new();
        private PoolObject _activityPoolObject;
        
        public IActivity CurrentActivity => _activityPoolObject.Convert<ActivityUIBehaviourBase>().Obj;
        
        /// <summary>
        /// 活动选项容器
        /// </summary>
        public RectTransform SvActivityContent => svActivity.content;
        
        /// <summary>
        /// 活动选项ToggleGroup
        /// </summary>
        public ToggleGroup ActivityGroup => ActivityContent;
        
        public void CacheActivityUI(PoolObject actvityUi)
        {
            _actvityUis.Add(actvityUi);
        }

        public ActivityUI GetFirstActivityUI()
        {
            return _actvityUis.Count > 0 ? _actvityUis[0].Convert<ActivityUI>().Obj : null;
        }

        public void UpdateActivityDetailUI(PoolObject actvity)
        {
            _activityPoolObject.Collect();
            _activityPoolObject = actvity;
        }
        
        public override void Destroy()
        {
            _activityPoolObject.Collect();
        }
    }
}
