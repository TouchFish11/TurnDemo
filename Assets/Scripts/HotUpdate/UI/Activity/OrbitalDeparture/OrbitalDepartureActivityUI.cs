using System;
using Core.DI;
using Core.UI;
using HotUpdate.Base.Data;
using HotUpdate.Base.Manager;
using HotUpdate.UI.Activity.Base;
using HotUpdate.UI.Activity.Common;
using TMPro;
using UnityEngine.UI;

namespace HotUpdate.UI.Activity.OrbitalDeparture
{
    using Task = System.Threading.Tasks.Task;
    
    /// <summary>
    /// 星旅启航活动UI
    /// </summary>
    public class OrbitalDepartureActivityUI : ActivityUIBehaviourBase
    {
        [Inject] private IActivityDataProvider activityDataProvider;

        [InjectUI] private Image imgActivityBackground;
        [InjectUI] private TextMeshProUGUI txtActivityDescrition;
        [InjectUI] private TextMeshProUGUI txtActivityName;
        [InjectUI] private TextMeshProUGUI txtTime;
        
        [NonSerialized] public ActivityJoinComponent activityJoinComponent;
        private AwardPreviewComponent _awardPreviewComponent;

        public OrbitalDepartureHandler OrbitalDepartureHandler => activityContentHandler as OrbitalDepartureHandler;
        
        protected override void Awake()
        {
            base.Awake();
            activityJoinComponent = GetComponentInChildren<ActivityJoinComponent>();
            _awardPreviewComponent = GetComponentInChildren<AwardPreviewComponent>();
        }

        protected override async Task OnInit()
        {
            // 初始化组件
            activityJoinComponent.Init(this, activityInfo);
            _awardPreviewComponent.Init(this, activityInfo);
            
            // 初始化界面
            imgActivityBackground.sprite = await iconService.LoadIconAsync(activityInfo.f_bkUi_Res);
            txtActivityDescrition.text = $"{activityInfo.f_description}";
            txtActivityName.text = $"{activityInfo.f_name}";
            txtTime.text = $"{ToDurationStr(activityInfo.f_duration)}";
        }

        protected override async Task OnShow()
        {
            OrbitalDepartureHandler.UpdateShow();
            activityJoinComponent.OnClickJoin += OnTriggerJoin;
            // 解析奖励ID数组，获取物品格子
            var itemGrids = await itemService.CreateItemGrid(activityInfo.f_awardIds);
            _awardPreviewComponent.SetAwards(itemGrids);
        }

        private void OnTriggerJoin()
        {
            OrbitalDepartureHandler.ReceiveReward();
        }
        
        protected override Task OnHide()
        {
            activityJoinComponent.OnClickJoin -= OnTriggerJoin;
            
            itemService.Clear();
            iconService.ReleaseAll();
            return Task.CompletedTask;
        }

        protected override void OnDispose()
        {
            
        }
    }
}
