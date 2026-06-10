using System;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Scene;
using Core.Serialize.Binary;
using Core.UI.ViewController;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Service;
using HotUpdate.Base.UI;
using HotUpdate.Common.Config.ExcelInfo.Container;
using HotUpdate.Game.Activity.Core;

namespace HotUpdate.UI.Activity.Base
{
    /// <summary>
    /// 活动界面控制器
    /// </summary>
    public class ActivityController : UIController<ActivityView>, IBlockOperation
    {

        [Inject] private ObjectSpawner _objectSpawner;
        [Inject] private IActivityDataManager _activityDataManager;
        [Inject] private ISceneManager _sceneManager;
        [Inject] private IActivityDataFactory _activityDataFactory;
        [Inject] private IUIService _uiService;
        [Inject] private IBinaryDataManager _binaryDataManager;
        [Inject] private IIconService _iconService;

        public bool BlockOperation { get; } = true;
        
        protected override bool IsCursorVisible { get; set; } = true;

        protected override Task OnInit()
        {
            return Task.CompletedTask;
        }

        protected override async Task OnActive()
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
                view.CacheActivityUI(activityUI);
            }
            
            // 默认选中第一个UI
            view.GetFirstActivityUI().SelectActivity();
        }

        protected override Task OnInactivate()
        {
            _objectSpawner.Dispose();
            // 显示主界面
            return _uiService.ShowAsync(_uiService.GetPanel(EUIPanelId.MainPanel).PanelId);
        }
        
        protected override void OnButtonClick(string btnName)
        {
            switch (btnName)
            {
                case nameof(view.btnClose):
                    uiManager.DestroyView(panelId);
                    break;
            }
        }

        public async void UpdateDetailActivity(int selectId)
        {
            if (view.CurrentActivity != null && selectId == view.CurrentActivity.ActivityId)
                return;
            
            // 获取活动配置
            var activityInfo = _binaryDataManager.GetConfig<ActivityInfoContainer>(EConfigLoadType.Excel).dataDic[selectId];
            // 活动本地活动数据
            var activityDataCollection = _activityDataManager.ActivityDataCollection as ActivityDataCollection;
            var activityUIBehaviourBase = await _objectSpawner.SpawnAsync<ActivityUIBehaviourBase>(activityInfo.f_detailUI_res, view.ActivityDetailArea);
            // 初始化详细界面
            if (!activityDataCollection.TryGetValue(activityInfo.f_id, out var activityData))
            {
                // 新增活动数据
                activityData = _activityDataFactory.tryGetData(activityInfo.f_id, out var data) ? data : null;
                if (activityData == null)
                    throw new NullReferenceException($"activityData {activityInfo.f_id} not found");
                
                // 初始化ID
                activityData.ActivityId = activityInfo.f_id;
                // 缓存新增数据
                activityDataCollection.TryAdd(activityInfo.f_id, activityData);
            }

            var handler = ActivityContentHandlerHelper.CreateHandler(activityData);
            // 初始化
            await activityUIBehaviourBase.Init(activityData.ActivityId, activityInfo, handler);
            // 更新界面
            view.UpdateActivityDetailUI(activityUIBehaviourBase, _objectSpawner);
        }

        protected override Task OnDestroy()
        {
            _objectSpawner = null;
            return Task.CompletedTask;
        }
    }
}
