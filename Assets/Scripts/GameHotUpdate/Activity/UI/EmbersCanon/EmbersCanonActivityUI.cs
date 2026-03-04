using System;
using System.Collections.Generic;
using Core.Log;
using GameHotUpdate.Activity.Core;
using GameHotUpdate.Activity.UI.Common;
using GameHotUpdate.Config;
using GameHotUpdate.Item;
using GameHotUpdate.Item.UI;

namespace GameHotUpdate.Activity.UI.EmbersCanon
{
    using Task = System.Threading.Tasks.Task;
    
    /// <summary>
    /// 余烬圣典活动UI
    /// </summary>
    public class EmbersCanonActivityUI : ActivityUIBehaviourBase
    {
        private ActivityBkComponent _activityBkComponent;
        private ActivityJoinComponent _activityJoinComponent;
        
        private ActivityDescritionComponent _activityDescritionComponent;
        private ActivityNameComponent _activityNameComponent;
        private ActivityTimeComponent _activityTimeComponent;
        
        private AwardPreviewComponent _awardPreviewComponent;
        private LimitTimeAwardComponent _limitTimeAwardComponent;

        private readonly IList<ItemGrid> _itemGrids = new List<ItemGrid>();
        
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
            var backGround = await spriteLoader.LoadSpriteAsync(AbKeyCollection.Spriteatlas, ResKeyCollection.Atlas_Activity,
                    activityInfo.f_bkUi_Res);
            _activityBkComponent.SetBackGround(backGround);

            _activityJoinComponent.OnClickJoin += OnTriggerJoin;
            _limitTimeAwardComponent.OnClickAward += OnTriggerLimitTimeAward;
            
            _activityDescritionComponent.SetActivityDescrition(out var activityDescrition);
            activityDescrition.text = $"{activityInfo.f_description}";
            
            _activityNameComponent.SetTitle(out var txtActivityName);
            txtActivityName.text = $"{activityInfo.f_name}";
            
            _activityTimeComponent.SetDurationTime(out var txtTime);
            txtTime.text = $"{ToDurationStr(activityInfo.f_duration)}";
            
            // 解析奖励ID数组，获取物品格子
            ItemUtility.GetItemGrid(activityInfo.f_awardIds, grid =>
            {
                if (grid != null)
                {
                    _awardPreviewComponent.SetAward(grid);
                    _itemGrids.Add(grid);
                }
            });
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
                var subActivityUi = await prefabLoader.GetObjectAsync<EmbersCanonSubActivityUI_01>(AbKeyCollection.Ui, ResKeyCollection.EmbersCanonSubActivityUI_01,
                    activityView);
                // 初始化关卡子界面
                subActivityUi.Init(ActivityData, activityInfo);
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(EmbersCanonActivityUI)}.{nameof(OnTriggerJoin)}：{e.Message}，{e.StackTrace}");
            }
        }

        private void OnTriggerLimitTimeAward()
        {
            LogManager.Log($"限时奖励按钮点击");
        }

        /// <summary>
        /// 清理物品UI
        /// </summary>
        private void ClearItem()
        {
            foreach (var itemGrid in _itemGrids)
            {
                prefabLoader.CollectAsset(itemGrid.gameObject);
            }
            _itemGrids.Clear();
            prefabLoader.RealseAsset(AbKeyCollection.Ui, ResKeyCollection.ItemGrid);
        }
        
        protected override void OnHide()
        {
            ClearItem();
            prefabLoader.RealseAsset(AbKeyCollection.Ui, ResKeyCollection.EmbersCanonActivityUI);
            
            _activityJoinComponent.OnClickJoin -= OnTriggerJoin;
            _limitTimeAwardComponent.OnClickAward -= OnTriggerLimitTimeAward;
        }
    }
}
