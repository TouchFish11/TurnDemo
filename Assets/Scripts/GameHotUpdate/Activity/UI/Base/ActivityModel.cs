using System;
using System.Collections.Generic;
using Core.Pool;
using Core.Service;
using Core.UI.MVC;
using GameHotUpdate.Activity.Core;

namespace GameHotUpdate.Activity.UI.Base
{
    /// <summary>
    /// 活动界面数据
    /// </summary>
    public class ActivityModel : UIModel
    {
        // 活动选项UI列表
        private List<ActivityUI> activityUis = new();
        // 活动界面类型缓存
        private HashSet<Type> activityTypes = new();
        
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
                // 清除当前缓存的界面
                ServiceLocator.Get<IPoolManager>().PushObj(Activity.GameObject);
            }
            Activity = currentActivity;
            activityTypes.Add(type);
        }

        /// <summary>
        /// 获取活动类型
        /// 用于清理缓存池
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> GetActivityTypes()
        {
            foreach (var activityType in activityTypes)
            {
                yield return activityType;
            }
        }

        public override void ClearData()
        {
            Activity = null;
            activityUis = null;
            activityTypes = null;
            base.ClearData();
        }
    }
}