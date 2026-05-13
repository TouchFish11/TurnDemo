using System;
using Core.AssetBundles.Management;
using Core.DI;
using HotUpdate.Common;
using HotUpdate.Common.Item;
using HotUpdate.Game.Activity.Core;
using HotUpdate.Game.Activity.UI.Common;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Activity.UI.EmbersCanon
{
    using Task = System.Threading.Tasks.Task;
    
    /// <summary>
    /// 余烬圣典活动UI
    /// </summary>
    public class EmbersCanonActivityUI : ActivityUIBehaviourBase
    {
        [Inject] private ItemService _itemService;
        
        private ActivityBkComponent _activityBkComponent;
        private ActivityJoinComponent _activityJoinComponent;
        
        private ActivityDescritionComponent _activityDescritionComponent;
        private ActivityNameComponent _activityNameComponent;
        private ActivityTimeComponent _activityTimeComponent;
        
        private AwardPreviewComponent _awardPreviewComponent;
        private LimitTimeAwardComponent _limitTimeAwardComponent;
        
        protected override void Awake()
        {
            base.Awake();
            _activityBkComponent = GetComponentInChildren<ActivityBkComponent>();
            _activityJoinComponent = GetComponentInChildren<ActivityJoinComponent>();
            _activityDescritionComponent = GetComponentInChildren<ActivityDescritionComponent>();
            _activityNameComponent = GetComponentInChildren<ActivityNameComponent>();
            _activityTimeComponent = GetComponentInChildren<ActivityTimeComponent>();
            _awardPreviewComponent = GetComponentInChildren<AwardPreviewComponent>();
            _limitTimeAwardComponent = GetComponentInChildren<LimitTimeAwardComponent>();
        }
        
        protected override async Task OnInit()
        {
            await base.OnInit();
            // 初始化界面背景
            using var backGround = await GameAsset.LoadAssetAsync<Sprite>(activityInfo.f_bkUi_Res);
            _activityBkComponent.SetBackGround(backGround.Asset);

            _activityJoinComponent.OnClickJoin += OnTriggerJoin;
            _limitTimeAwardComponent.OnClickAward += OnTriggerLimitTimeAward;
            
            _activityDescritionComponent.SetActivityDescrition(out var activityDescrition);
            activityDescrition.text = $"{activityInfo.f_description}";
            
            _activityNameComponent.SetTitle(out var txtActivityName);
            txtActivityName.text = $"{activityInfo.f_name}";
            
            _activityTimeComponent.SetDurationTime(out var txtTime);
            txtTime.text = $"{ToDurationStr(activityInfo.f_duration)}";
            
            // 解析奖励ID数组，获取物品格子
            _itemService.GetItemGrid(activityInfo.f_awardIds, null);
        }

        protected override Task OnShow()
        {
            return Task.CompletedTask;
        }

        private async void OnTriggerJoin()
        {
            try
            {
                // 创建关卡界面到活动界面下
                var subActivityUi = await _objectSpawner.SpawnAsync<EmbersCanonSubActivityUI_01>(ResKeyCollection.EmbersCanonSubActivityUI_01,
                    activityView);
                // 初始化关卡子界面
                subActivityUi.Obj.Init(ActivityData, activityInfo);
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(EmbersCanonActivityUI)}.{nameof(OnTriggerJoin)}：{e.Message}，{e.StackTrace}");
            }
        }

        private void OnTriggerLimitTimeAward()
        {
            Logger.Log($"限时奖励按钮点击");
        }
        
        
        protected override void OnHide()
        {
            _itemService.Dispose();
            _objectSpawner.Dispose();
            _activityJoinComponent.OnClickJoin -= OnTriggerJoin;
            _limitTimeAwardComponent.OnClickAward -= OnTriggerLimitTimeAward;
        }
    }
}
