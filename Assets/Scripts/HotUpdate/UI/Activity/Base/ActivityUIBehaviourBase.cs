using System;
using Core.AssetBundles.Management;
using Core.DI;
using Core.UI;
using Core.Utility;
using HotUpdate.Common.Config.ExcelInfo.Info;
using HotUpdate.Game.Activity.Core;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.UI.Activity.Base
{
    using Task = System.Threading.Tasks.Task;
    
    /// <summary>
    /// 活动基类
    /// </summary>
    public abstract class ActivityUIBehaviourBase : UIBehaviourBase, IActivity
    {
        [Inject] protected ObjectSpawner _objectSpawner;
        
        public GameObject GameObject { get; private set; }
        public int ActivityId { get; private set; }

        // 活动信息
        protected ActivityInfo activityInfo;
        // 活动界面父对象
        protected Transform activityView;

        protected override void Awake()
        {
            base.Awake();
            GameObject = gameObject;
        }

        protected sealed override async void OnEnable()
        {
            await OnShow();
        }

        public async void Init(int activityId, ActivityInfo activityInfo)
        {
            try
            {
                ActivityId = activityId;
                this.activityInfo = activityInfo;
                activityView = transform.GetComponentInParent<ActivityView>().transform;
                // 初始化活动界面
                await OnInit();
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(ActivityUIBehaviourBase)}.{nameof(Init)}：{e.Message}");
            }
        }

        /// <summary>
        /// 在初始化时执行
        /// 会执行OnShow
        /// </summary>
        /// <returns></returns>
        protected virtual Task OnInit()
        {
            return OnShow();
        }

        /// <summary>
        /// 在活动界面显示时执行
        /// </summary>
        /// <returns></returns>
        protected abstract Task OnShow();

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
