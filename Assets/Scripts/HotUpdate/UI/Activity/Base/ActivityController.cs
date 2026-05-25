using System;
using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Reflection;
using Core.Scene;
using Core.Serialize.Binary;
using Core.UI.ViewController;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Scene;
using HotUpdate.Common;
using HotUpdate.Common.Config.Activity;
using HotUpdate.Common.Config.ExcelInfo.Container;
using HotUpdate.Common.Config.ExcelInfo.Info;
using HotUpdate.Game.Activity.Core;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Turn;
using HotUpdate.Game.Main;
using UnityEngine;

namespace HotUpdate.UI.Activity.Base
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 活动界面控制器
    /// </summary>
    public class ActivityController : UIController<ActivityView>
    {
        [Inject] private ISceneGenerator _sceneGenerator;
        [Inject] private ObjectSpawner _objectSpawner;
        [Inject] private IActivityDataManager _activityDataManager;
        [Inject] private IBattleManager _battleManager;
        [Inject] private IPlayerManager _playerManager;
        [Inject] private ISceneManager _sceneManager;
        
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
        
        public async Task EnterActivityBattle(BattleConfigEntry configEntry, int activityId, Action onLevelComplete)
        {
            var turnData = new TurnData
            {
                TotalTurnNumber = configEntry.battleWave,
                Waves = new List<List<int>> { configEntry.monsterIds }
            };

            await _battleManager.EnterBattle(turnData,
                OnPreEnter: async () =>
                {
                    _sceneGenerator.ClearMainScene();
                    await uiManager.SetViewActive(panelId, false);
                },
                onBattleOver: async () =>
                {
                    await ChangedScene();
                    await _playerManager.CreatePlayer(1001);
                    onLevelComplete?.Invoke();
                    if(_activityDataManager.TryGetData(activityId, out var activityData))
                        activityData.CurrentPro += 1;
                    await uiManager.SetViewActive(panelId, true);
                });
        }

        private async Task ChangedScene()
        {
            await _sceneManager.LoadSceneAsync(ResKeyCollection.MainScene, UnityEngine.SceneManagement.LoadSceneMode.Single, null);
            await _sceneGenerator.InitMainScene();
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
            if (view.CurrentActivity != null && activityInfo.f_id == view.CurrentActivity.ActivityId)
                return;
            
            // 活动本地活动数据
            var activityDataCollection = _activityDataManager.ActivityDataCollection as ActivityDataCollection;
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

            poolObject.Obj.Init(activityData.ActivityId, activityInfo);
            // 缓存界面
            view.UpdateActivityDetailUI(poolObject);
        }
    }
}
