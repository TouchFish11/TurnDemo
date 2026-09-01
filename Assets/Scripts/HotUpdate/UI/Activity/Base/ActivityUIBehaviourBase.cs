using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.UI;
using Core.Utility;
using HotUpdate.Base.Service;
using HotUpdate.UI.Items;
using UnityEngine;

namespace HotUpdate.UI.Activity.Base
{
    /// <summary>
    /// 活动基类
    /// </summary>
    public abstract class ActivityUIBehaviourBase : UIBehaviourBase, IActivity
    {
        [Inject] protected ObjectSpawner objectSpawner;
        [Inject] protected ItemService itemService;
        [Inject] protected IIconService iconService;
        
        private readonly Stack<IActivitySubView> _subViews = new();
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

        public async Task Init(int activityId, ActivityInfo activityInfo, IActivityContentHandler contentHandler)
        {
            ActivityId = activityId;
            this.activityInfo = activityInfo;
            contentHandler.Init(this);
            activityContentHandler = contentHandler;
            activityView = transform.GetComponentInParent<ActivityView>().transform;
            // 初始化活动界面
            await OnInit();
            await Show();
        }

        public async Task Show()
        {
            await OnShow();
            // 恢复子界面：栈底 → 栈顶，回到离开时的界面
            foreach (var scViewreen in _subViews.Reverse())
            {
                await scViewreen.Show();
            }
        }

        public async Task Hide()
        {
            foreach (var scViewreen in _subViews)
            {
                await scViewreen.Hide();
            }
            await OnHide();
        }

        public async Task Destroy()
        {
            await Hide();
            while (_subViews.TryPop(out var subView))
            {
                await subView.Destroy();
            }
            OnDispose();
            objectSpawner.Dispose();
            objectSpawner = null;
            itemService.Dispose();
            itemService = null;
            iconService.Dispose();
            iconService = null;
        }
        
        /// <summary>
        /// 打开一个子界面（压栈并显示）
        /// </summary>
        protected async Task PushSubView(IActivitySubView subView)
        {
            _subViews.Push(subView);
            await subView.Show();
        }
        
        /// <summary>
        /// 关闭栈顶子界面（弹栈并销毁）
        /// </summary>
        protected async Task<IActivitySubView> PopSubView()
        {
            if (_subViews.Count <= 0) 
                return null;
            
            var subView = _subViews.Pop();
            await subView.Destroy();
            return subView;
        }

        /// <summary>
        /// 仅在第一次创建对象时执行
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
        protected abstract Task OnHide();

        /// <summary>
        /// 当界面被销毁时执行
        /// </summary>
        protected virtual void OnDispose()
        {
            
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
