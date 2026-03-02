using System;
using Core.Loader.Sprite;
using Core.Loader.UI;
using Core.Log;
using Core.Pool;
using Core.Service;
using Core.UI;
using Core.Utility;
using GameHotUpdate.Activity.Data;
using GameHotUpdate.Activity.UI.Base;
using UnityEngine;

namespace GameHotUpdate.Activity.Core
{
    using Task = System.Threading.Tasks.Task;
    
    /// <summary>
    /// 活动基类
    /// </summary>
    public abstract class ActivityUIBehaviourBase : UIBehaviourBase, IActivity
    {
        protected IPoolManager poolManager;
        protected IUiLoader uiLoader; 
        protected ISpriteLoader spriteLoader;
        
        public GameObject GameObject { get; private set; }
        public ActivityData ActivityData { get; private set; }

        // 活动信息
        protected ActivityInfo activityInfo;
        // 活动界面父对象
        protected Transform activityView;

        protected override void Awake()
        {
            base.Awake();
            GameObject = gameObject;
            poolManager = ServiceLocator.Get<IPoolManager>();
            uiLoader = ServiceLocator.Get<IUiLoader>();
            spriteLoader = ServiceLocator.Get<ISpriteLoader>();
        }

        protected sealed override async void OnEnable()
        {
            if (ActivityData == null || activityInfo == null)
            {
                return;
            }
            
            await OnShow();
        }

        public async void Init(ActivityData activityData, ActivityInfo activityInfo)
        {
            try
            {
                ActivityData = activityData;
                this.activityInfo = activityInfo;
                activityView = transform.GetComponentInParent<ActivityView>().transform;
                // 初始化活动界面
                await OnInit();
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(ActivityUIBehaviourBase)}.{nameof(Init)}：{e.Message}");
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
