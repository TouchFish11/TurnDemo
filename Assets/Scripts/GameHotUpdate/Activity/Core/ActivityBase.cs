using System;
using System.Threading.Tasks;
using Core.Log;
using GameHotUpdate.Activity.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameHotUpdate.Activity.Core
{
    /// <summary>
    /// 活动基类
    /// </summary>
    public abstract class ActivityBase : UIBehaviour, IActivity
    {
        public GameObject GameObject { get; private set; }

        public ActivityData ActivityData { get; private set; }

        // 活动信息
        protected ActivityInfo activityInfo;

        protected override void Awake()
        {
            GameObject = this.gameObject;
        }
        
        public async void Init(ActivityData activityData, ActivityInfo activityInfo)
        {
            try
            {
                ActivityData = activityData;
                this.activityInfo = activityInfo;
                // 初始化具体活动界面
                await OnInit();
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(ActivityBase)}.{nameof(Init)}：{e.Message}");
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
    }
}
