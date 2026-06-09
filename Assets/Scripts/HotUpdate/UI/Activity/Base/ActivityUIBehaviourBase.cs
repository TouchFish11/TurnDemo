using System;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.UI;
using Core.Utility;
using HotUpdate.Common.Config.ExcelInfo.Info;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.UI.Activity.Base
{
    /// <summary>
    /// 活动基类
    /// </summary>
    public abstract class ActivityUIBehaviourBase : UIBehaviourBase, IActivity
    {
        [Inject] protected ObjectSpawner _objectSpawner;

        // 活动信息
        protected ActivityInfo activityInfo;
        // 活动界面父对象
        protected Transform activityView;
        // 活动内容处理器
        protected IActivityContentHandler activityContentHandler;
        
        public GameObject GameObject { get; private set; }
        
        public int ActivityId { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();
            GameObject = gameObject;
        }

        protected sealed override async void OnEnable()
        {
            try
            {
                await OnShow();
            }
            catch (Exception e)
            {
                Logger.LogError($"[{nameof(ActivityUIBehaviourBase)}]: Activity show error,{e.Message}");
            }
        }

        public async Task Init(int activityId, ActivityInfo activityInfo, IActivityContentHandler contentHandler)
        {
            ActivityId = activityId;
            this.activityInfo = activityInfo;
            contentHandler.Init(this);
            activityContentHandler = contentHandler;
            activityView = transform.GetComponentInParent<ActivityView>().transform;
            // 初始化活动界面
            await OnInit();
            await OnShow();
        }

        /// <summary>
        /// 在初始化时执行
        /// 会执行OnShow
        /// </summary>
        /// <returns></returns>
        protected abstract Task OnInit();

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
