using System;
using System.Threading.Tasks;
using Core.Log;
using Core.UI;
using HotUpdate.UI.Activity.Base;
using HotUpdate.UI.Activity.Common;
using TMPro;
using UnityEngine.UI;
using Logger = Core.Log.Logger;

namespace HotUpdate.UI.Activity.EmbersCanon
{
    /// <summary>
    /// 余烬圣典活动UI
    /// </summary>
    public class EmbersCanonActivityUI : ActivityUIBehaviourBase
    {
        [InjectUI] private Image imgActivityBackground;
        [InjectUI] private TextMeshProUGUI txtActivityDescrition;
        [InjectUI] private TextMeshProUGUI txtActivityName;
        [InjectUI] private TextMeshProUGUI txtTime;
        
        private ActivityJoinComponent _activityJoinComponent;
        private AwardPreviewComponent _awardPreviewComponent;
        private LimitTimeAwardComponent _limitTimeAwardComponent;
        
        private EmbersCanonSubActivityUI_01 _embersCanonSubActivityUI_01;
        
        public EmbersCanonHandler EmbersCanonHandler => activityContentHandler as EmbersCanonHandler;
        
        protected override void Awake()
        {
            base.Awake();
            _activityJoinComponent = GetComponentInChildren<ActivityJoinComponent>();
            _awardPreviewComponent = GetComponentInChildren<AwardPreviewComponent>();
            _limitTimeAwardComponent = GetComponentInChildren<LimitTimeAwardComponent>();
        }
        
        protected override async Task OnInit()
        {
            // 初始化界面
            imgActivityBackground.sprite = await iconService.LoadIconAsync(activityInfo.f_bkUi_Res);
            txtActivityDescrition.text = activityInfo.f_description;
            txtActivityName.text = activityInfo.f_name;
            txtTime.text = ToDurationStr(activityInfo.f_duration);
        }

        protected override async Task OnShow()
        {
            _activityJoinComponent.OnClickJoin += OnTriggerJoin;
            _limitTimeAwardComponent.OnClickAward += OnTriggerLimitTimeAward;
            // 解析奖励ID数组，获取物品格子
            var itemGrids = await itemService.CreateItemGrid(activityInfo.f_awardIds);
            _awardPreviewComponent.SetAwards(itemGrids);
            
            if(_embersCanonSubActivityUI_01)
                await _embersCanonSubActivityUI_01.Activate();
        }

        private async void OnTriggerJoin()
        {
            try
            {
                // 创建关卡界面到活动界面下
                _embersCanonSubActivityUI_01 = await objectSpawner.SpawnAsync<EmbersCanonSubActivityUI_01>(AssetKeys.EmbersCanonSubActivityUI_01, activityView);
                // 初始化关卡子界面
                await _embersCanonSubActivityUI_01.Init(activityInfo, EmbersCanonHandler);
                _embersCanonSubActivityUI_01.OnClose += OnSubViewClose;
            }
            catch (Exception e)
            {
                Logger.LogError(ELogTags.Activity, $"{nameof(EmbersCanonActivityUI)}: Join activity error,{e.Message}");
            }
        }

        private void OnTriggerLimitTimeAward()
        {
            Logger.LogDebug(ELogTags.Activity, $"限时奖励按钮点击");
        }

        private void OnSubViewClose()
        {
            _embersCanonSubActivityUI_01?.Deactivate();
            objectSpawner.Release(_embersCanonSubActivityUI_01);
            _embersCanonSubActivityUI_01 = null;
        }
        
        protected override Task OnHide()
        {
            _activityJoinComponent.OnClickJoin -= OnTriggerJoin;
            _limitTimeAwardComponent.OnClickAward -= OnTriggerLimitTimeAward;
            
            itemService.Clear();
            iconService.ReleaseAll();
            
            _embersCanonSubActivityUI_01?.Deactivate();
            return Task.CompletedTask;
        }

        protected override void OnDispose()
        {
            _embersCanonSubActivityUI_01.Destroy();
        }
    }
}
