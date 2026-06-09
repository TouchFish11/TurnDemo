using System;
using System.Threading.Tasks;
using Core.DI;
using Core.UI;
using HotUpdate.Base.Service;
using HotUpdate.UI.Activity.Base;
using HotUpdate.UI.Activity.Common;
using HotUpdate.UI.Item;
using TMPro;
using Logger = Core.Log.Logger;

namespace HotUpdate.UI.Activity.EmbersCanon
{
    /// <summary>
    /// 余烬圣典活动UI
    /// </summary>
    public class EmbersCanonActivityUI : ActivityUIBehaviourBase
    {
        [Inject] private ItemService _itemService;
        [Inject] private IIconService _iconService;

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
            // 初始化界面背景
            await _iconService.LoadIconAsync(activityInfo.f_bkUi_Res);

            _activityJoinComponent.OnClickJoin += OnTriggerJoin;
            _limitTimeAwardComponent.OnClickAward += OnTriggerLimitTimeAward;

            txtActivityDescrition.text = activityInfo.f_description;
            txtActivityName.text = activityInfo.f_name;
            txtTime.text = ToDurationStr(activityInfo.f_duration);
            
            // 解析奖励ID数组，获取物品格子
            var itemGrids = await _itemService.CreateItemGrid(activityInfo.f_awardIds);
            _awardPreviewComponent.SetAwards(itemGrids);
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
                _embersCanonSubActivityUI_01 = await _objectSpawner.SpawnAsync<EmbersCanonSubActivityUI_01>(AssetKeys.EmbersCanonSubActivityUI_01, activityView);
                // 初始化关卡子界面
                await _embersCanonSubActivityUI_01.Init(activityInfo, EmbersCanonHandler);
                _embersCanonSubActivityUI_01.OnClose += OnSubViewClose;
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(EmbersCanonActivityUI)}: Join activity error,{e.Message}");
            }
        }

        private void OnTriggerLimitTimeAward()
        {
            Logger.Log($"限时奖励按钮点击");
        }

        private void OnSubViewClose()
        {
            _objectSpawner.Release(_embersCanonSubActivityUI_01);
        }
        
        
        protected override void OnHide()
        {
            _itemService.Dispose();
            _objectSpawner.Dispose();
            _iconService.ReleaseAll();
            _activityJoinComponent.OnClickJoin -= OnTriggerJoin;
            _limitTimeAwardComponent.OnClickAward -= OnTriggerLimitTimeAward;
        }
    }
}
