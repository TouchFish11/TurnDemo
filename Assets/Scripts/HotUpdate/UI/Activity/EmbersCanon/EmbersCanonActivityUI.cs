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
            var itemGrids = await itemService.CreateItemGrids(activityInfo.f_awardIds);
            _awardPreviewComponent.SetAwards(itemGrids);
        }

        private async void OnTriggerJoin()
        {
            try
            {
                // 创建关卡界面到活动界面下
                var subView = await objectSpawner.SpawnAsync<EmbersCanonSubActivityUI_01>(AssetKeys.EmbersCanonSubActivityUI_01, activityView);
                // 初始化关卡子界面
                subView.Init(activityInfo, EmbersCanonHandler);
                subView.OnClose += OnSubViewClose;
                // 压栈 + 显示
                await PushSubView(subView);
            }
            catch (Exception e)
            {
                Logger.LogException(ELogTags.Activity, e);
            }
        }

        private void OnTriggerLimitTimeAward()
        {
            Logger.LogDebug(ELogTags.Activity, $"限时奖励按钮点击");
        }

        private async void OnSubViewClose()
        {
            try
            {
                // 弹栈 + 销毁
                var subView = await PopSubView();
                if(subView != null)
                    objectSpawner.Release((EmbersCanonSubActivityUI_01)subView, true);
            }
            catch (Exception e)
            {
                Logger.LogException(ELogTags.Activity, e);
            }
        }
        
        protected override Task OnHide()
        {
            _activityJoinComponent.OnClickJoin -= OnTriggerJoin;
            _limitTimeAwardComponent.OnClickAward -= OnTriggerLimitTimeAward;
            itemService.Clear();
            iconService.ReleaseAll();
            return Task.CompletedTask;
        }
    }
}
