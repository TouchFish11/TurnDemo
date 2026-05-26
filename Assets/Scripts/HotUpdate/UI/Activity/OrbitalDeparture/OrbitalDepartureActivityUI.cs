using Core.AssetBundles.Management;
using Core.DI;
using Core.UI;
using HotUpdate.Activity.UI.Common;
using HotUpdate.Base.Manager;
using HotUpdate.UI.Activity.Base;
using HotUpdate.UI.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Activity.UI.OrbitalDeparture
{
    using Task = System.Threading.Tasks.Task;
    
    /// <summary>
    /// 星旅启航活动UI
    /// </summary>
    public class OrbitalDepartureActivityUI : ActivityUIBehaviourBase
    {
        [Inject] private ItemService _itemService;
        [Inject] private IActivityDataManager _activityDataManager;

        [InjectUI] private Image imgActivityBackground;
        [InjectUI] private TextMeshProUGUI txtActivityDescrition;
        [InjectUI] private TextMeshProUGUI txtActivityName;
        [InjectUI] private TextMeshProUGUI txtTime;
        
        private ActivityJoinComponent _activityJoinComponent;
        private AwardPreviewComponent _awardPreviewComponent;

        protected override void Awake()
        {
            base.Awake();
            _activityJoinComponent = GetComponentInChildren<ActivityJoinComponent>();
            _awardPreviewComponent = GetComponentInChildren<AwardPreviewComponent>();
        }

        protected override async Task OnInit()
        {
            await base.OnInit();
            
            // 初始化组件
            _activityJoinComponent.Init(this, activityInfo);
            _awardPreviewComponent.Init(this, activityInfo);
            
            // 初始化界面背景
            using var handle = await GameAsset.LoadAssetAsync<Sprite>(activityInfo.f_bkUi_Res);
            // 设置界面背景
            imgActivityBackground.sprite = handle.Asset;
            
            UpdateBtnJoin();
            // 监听按钮
            _activityJoinComponent.OnClickJoin += OnTriggerJoin;
            txtActivityDescrition.text = $"{activityInfo.f_description}";
            txtActivityName.text = $"{activityInfo.f_name}";
            txtTime.text = $"{ToDurationStr(activityInfo.f_duration)}";
            // 解析奖励ID数组，获取物品格子
            _itemService.GetItemGrid(activityInfo.f_awardIds, null);
        }

        protected override Task OnShow()
        {
            return Task.CompletedTask;
        }

        private void OnTriggerJoin()
        {
            if (!_activityDataManager.TryGetData(ActivityId, out var activityData))
                return;
            
            if (!activityData.IsComplete)
            {
                activityData.CurrentPro += 1;
                _activityJoinComponent.SetTitle(out var txtJoin);
                txtJoin.text = $"已领取";
            }
        }
        
        /// <summary>
        /// 更新按钮显示
        /// </summary>
        private void UpdateBtnJoin()
        {
            if (!_activityDataManager.TryGetData(ActivityId, out var activityData))
                return;
            
            _activityJoinComponent.SetTitle(out var txtJoin);
            txtJoin.text = !activityData.IsComplete ? "立即领取" : "已领取";
        }
        
        protected override void OnHide()
        {
            _itemService.Dispose();
            _objectSpawner.Dispose();
            _activityJoinComponent.OnClickJoin -= OnTriggerJoin;
        }
    }
}
