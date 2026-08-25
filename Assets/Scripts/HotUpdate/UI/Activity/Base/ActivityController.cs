using System;
using System.Reflection;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Exceptions;
using Core.Scene;
using Core.Serialize.Binary;
using Core.UI.ViewController;
using HotUpdate.Base.Data;
using HotUpdate.Base.Service;
using HotUpdate.Base.UI;
using HotUpdate.Game.Activity.Core;

namespace HotUpdate.UI.Activity.Base
{
    /// <summary>
    /// 活动界面控制器
    /// </summary>
    public class ActivityController : UIController<ActivityView>, IBlockOperation
    {
        [Inject] private ObjectSpawner _objectSpawner;
        [Inject] private IActivityDataProvider activityDataProvider;
        [Inject] private ISceneManager _sceneManager;
        [Inject] private IActivityDataFactory _activityDataFactory;
        [Inject] private IUIService _uiService;
        [Inject] private IBinaryDataManager _binaryDataManager;
        [Inject] private IIconService _iconService;

        public bool BlockOperation => true;

        protected override bool IsCursorVisible { get; set; } = true;

        protected override async Task OnInit()
        {
            // 读取活动数据
            var infoDic = _binaryDataManager.GetConfig<ActivityInfoContainer>(EConfigLoadType.Excel).dataDic;
            // 创建UI
            foreach (var activityInfo in infoDic.Values)
            {
                var activityUI = await _objectSpawner.SpawnAsync<ActivityUI>(AssetKeys.ActivityUI, view.SvActivityContent);
                // 加载图标
                var sprite = await _iconService.LoadIconAsync(activityInfo.f_bkUi_Res);
                // 初始化UI
                activityUI.Init(sprite, activityInfo, view.ActivityGroup);
                activityUI.OnSelect += UpdateDetailActivity;
                // 缓存UI
                view.ActvityUis.Add(activityUI);
            }
        }

        protected override async Task OnActive()
        {
            // 默认选中第一个UI
            await view.GetFirstActivityUI().SelectActivity();
        }

        protected override async Task OnInactivate()
        {
            // 执行子界面的失活逻辑
            if (view.CurrentActivity != null)
            {
                await view.CurrentActivity.Hide();
            }
        }
        
        protected override void OnButtonClick(string btnName)
        {
            switch (btnName)
            {
                case nameof(view.btnClose):
                    _uiService.CloseAsync(panelId, true, true);
                    break;
            }
        }

        /// <summary>
        /// 更新活动详细界面
        /// </summary>
        /// <param name="selectId"></param>
        /// <exception cref="NullReferenceException"></exception>
        public async Task UpdateDetailActivity(int selectId)
        {
            // 重复触发不重复执行
            if (view.CurrentActivity != null && selectId == view.CurrentActivity.ActivityId)
                return;
            
            // 先隐藏当前显示活动界面
            if(view.CurrentActivity != null)
                await view.CurrentActivity.Hide();
            
            // 获取活动配置
            var activityInfo = _binaryDataManager.GetConfig<ActivityInfoContainer>(EConfigLoadType.Excel).dataDic[selectId];
            // 活动本地活动数据
            var activityDataCollection = activityDataProvider.ActivityDataCollection as ActivityDataCollection;
            var activityUIBehaviourBase = await _objectSpawner.SpawnAsync<ActivityUIBehaviourBase>(activityInfo.f_detailUI_res, view.ActivityDetailArea);
            // 初始化详细界面
            if (!activityDataCollection.TryGetValue(activityInfo.f_id, out var activityData))
            {
                // 新增活动数据
                activityData = _activityDataFactory.tryGetData(activityInfo.f_id, out var data) ? data : null;
                if (activityData == null)
                    throw ExceptionHelper.Throw($"activityData {activityInfo.f_id} not found");
                
                // 初始化ID
                activityData.ActivityId = activityInfo.f_id;
                // 缓存新增数据
                activityDataCollection.TryAdd(activityInfo.f_id, activityData);
            }

            var handler = CreateHandler(activityData);
            // 初始化
            await activityUIBehaviourBase.Init(activityData.ActivityId, activityInfo, handler);
            // 更新界面
            if(view.CurrentActivity is ActivityUIBehaviourBase currentActivity)
                _objectSpawner.Release(currentActivity);
            view.CurrentActivity = activityUIBehaviourBase;
        }

        /// <summary>
        /// 创建活动内容处理器
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static IActivityContentHandler CreateHandler(ActivityData data)
        {
            var type = data.GetType();
            var attribute = type.GetCustomAttribute<ActivityIdAttribute>();
            if(attribute == null)
                return null;

            return DIContainer.Create(attribute.ActivityContentHandler) as IActivityContentHandler;
        }
        
        protected override async Task OnDispose()
        {
            _objectSpawner.Dispose();
            _objectSpawner = null;
            if (view.CurrentActivity != null)
            {
                await view.CurrentActivity.Destroy();
            }
        }
    }
}
