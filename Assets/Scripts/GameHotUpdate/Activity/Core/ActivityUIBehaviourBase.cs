using System;
using System.Threading.Tasks;
using Core.Log;
using Core.UI;
using Core.Utility;
using GameHotUpdate.Activity.Data;
using GameHotUpdate.Activity.UI.Base;
using UnityEngine;

namespace GameHotUpdate.Activity.Core
{
    /// <summary>
    /// 活动基类
    /// </summary>
    public abstract class ActivityUIBehaviourBase : UIBehaviourBase, IActivity
    {
        public GameObject GameObject { get; private set; }

        public ActivityData ActivityData { get; private set; }

        // 活动信息
        protected ActivityInfo activityInfo;
        // 活动界面父对象
        protected Transform activityView;

        protected override void Awake()
        {
            base.Awake();
            GameObject = this.gameObject;
        }
        
        public async void Init(ActivityData activityData, ActivityInfo activityInfo)
        {
            try
            {
                ActivityData = activityData;
                this.activityInfo = activityInfo;
                activityView = this.transform.GetComponentInParent<ActivityView>().transform;
                // 初始化具体活动界面
                await OnInit();
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(ActivityUIBehaviourBase)}.{nameof(Init)}：{e.Message}");
            }
        }

        /// <summary>
        /// 具体活动初始化
        /// </summary>
        /// <returns></returns>
        protected abstract Task OnInit();

        /// <summary>
        /// 在活动界面隐藏时执行
        /// 用于取消事件的监听
        /// </summary>
        protected abstract void OnHide();

        protected sealed override void OnDisable()
        {
            OnHide();
        }

        /// <summary>
        /// 剩余时间转字符串
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        protected static string ToDurationStr(int duration)
        {
            if (duration < 0)
            {
                return $"永久";
            }

            long seconds = duration * 24 * 60 * 60;
            return $"{TextUtility.SecondToHMS(seconds, "天", "小时", string.Empty, string.Empty)}";
        }
    }
}
