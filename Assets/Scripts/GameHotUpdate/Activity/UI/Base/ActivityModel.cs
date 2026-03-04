using System;
using System.Collections.Generic;
using Core.Loader.Object;
using Core.Service;
using Core.UI.MVC;
using GameHotUpdate.Activity.Core;
using GameHotUpdate.Config;

namespace GameHotUpdate.Activity.UI.Base
{
    /// <summary>
    /// 活动界面数据
    /// </summary>
    public class ActivityModel : UIModel
    {
        // 活动选项UI列表
        private readonly List<ActivityUI> activityUis = new();
        // 活动界面类型缓存
        private readonly HashSet<string> activityObjNames = new();
        
        /// <summary>
        /// 当前显示的活动界面缓存
        /// </summary>
        public IActivity Activity { get; private set; }

        /// <summary>
        /// 缓存活动选项UI
        /// </summary>
        /// <param name="activityUI"></param>
        public void CacheActivity(ActivityUI activityUI)
        {
            activityUis.Add(activityUI);
        }

        /// <summary>
        /// 获取第一个活动UI
        /// </summary>
        /// <returns></returns>
        public ActivityUI GetFirstActivityUI()
        {
            return activityUis[0];
        }

        /// <summary>
        /// 更新当前活动详细UI
        /// </summary>
        /// <param name="type"></param>
        /// <param name="currentActivity"></param>
        public void UpdateActivityDetailUI(Type type, IActivity currentActivity)
        {
            if (Activity != null)
            {
                // 释放资源
                ServiceLocator.Get<IPrefabLoader>().CollectAsset(Activity.GameObject);
            }
            Activity = currentActivity;
            activityObjNames.Add(currentActivity.GameObject.name);
        }
        
        public override void ClearData()
        {
            ServiceLocator.Get<IPrefabLoader>().CollectAsset(Activity.GameObject);
            Activity = null;
            foreach (var activityUi in activityUis)
            {
                ServiceLocator.Get<IPrefabLoader>().CollectAsset(activityUi.gameObject);
            }
            activityUis.Clear();
            ServiceLocator.Get<IPrefabLoader>().RealseAsset(AbKeyCollection.Ui, ResKeyCollection.ActivityUI);
            foreach (var name in activityObjNames)
            {
                ServiceLocator.Get<IPrefabLoader>().RealseAsset(AbKeyCollection.Ui, name);
            }
            activityObjNames.Clear();
            base.ClearData();
        }
    }
}