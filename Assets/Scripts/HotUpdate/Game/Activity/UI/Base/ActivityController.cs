using System;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Reflection;
using Core.Serialize.Binary;
using Core.UI.ViewController;
using HotUpdate.Base.Activity;
using HotUpdate.Base.Manager;
using HotUpdate.Common;
using HotUpdate.Common.Config.ExcelInfo.Container;
using HotUpdate.Common.Config.ExcelInfo.Info;
using HotUpdate.Game.Activity.Core;
using HotUpdate.Game.Activity.Data;
using UnityEngine;

namespace HotUpdate.Game.Activity.UI.Base
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 活动界面控制器
    /// </summary>
    public class ActivityController : UIController<ActivityView>
    {
        [Inject] private IGameManager _gameManager;
        [Inject] private ObjectSpawner _objectSpawner;
        private int mainControllerId;

        protected override Task OnInit()
        {
            return Task.CompletedTask;
        }

        protected override async Task OnActive()
        {
            // 读取活动数据
            var infoDic = DIContainer.GetInstance<IBinaryDataManager>().GetConfig<ActivityInfoContainer>(EConfigLoadType.Excel).dataDic;
            // 创建UI
            foreach (var activityInfo in infoDic.Values)
            {
                var poolObject = await _objectSpawner.SpawnAsync<ActivityUI>(ResKeyCollection.ActivityUI, view.SvActivityContent);
                var activityUI = poolObject.Obj;
                // 加载图标
                var handle = await GameAsset.LoadAssetAsync<Sprite>(activityInfo.f_bkUi_Res);
                // 初始化UI
                activityUI.Init(handle.Asset, activityInfo, view.ActivityGroup, this);
                // 缓存UI
                view.CacheActivityUI(poolObject);
            }
            
            // 默认选中第一个UI
            view.GetFirstActivityUI().SelectActivity();
        }

        protected override Task OnInactivate()
        {
            // 显示主界面
            return uiManager.SetViewActive(mainControllerId, true);
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

        public async Task UpdateDetailActivity(ActivityInfo activityInfo)
        {
            if (view.CurrentActivity != null && activityInfo.f_id == view.CurrentActivity.ActivityData.ActivityId)
                return;
            
            // 活动本地活动数据
            var activityDataCollection = _gameManager.GameDataManager.GetProvider<IActivityDataProvider>().ActivityDataCollection as ActivityDataCollection;
            if(activityDataCollection == null) 
                throw new NullReferenceException(nameof(activityDataCollection));

            var poolObject = await _objectSpawner.SpawnAsync<ActivityUIBehaviourBase>(activityInfo.f_detailUI_res, view.ActivityDetailArea);
            // 初始化详细界面
            if (!activityDataCollection.TryGetValue(activityInfo.f_id, out var activityData))
            {
                // 新增活动数据
                activityData = DIContainer.GetInstance<IFactoryManager>()
                    .GetFactory<IActivityDataFactory, ActivityDataFactory>()
                    .GetData(activityInfo.f_id);
                // 初始化ID
                activityData.ActivityId = activityInfo.f_id;
                // 缓存
                activityDataCollection.TryAdd(activityInfo.f_id, activityData);
            }

            poolObject.Obj.Init(activityData, activityInfo);
            // 缓存界面
            view.UpdateActivityDetailUI(poolObject);
        }
    }
}
