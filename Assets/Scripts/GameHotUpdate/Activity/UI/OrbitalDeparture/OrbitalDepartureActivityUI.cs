using System.Collections.Generic;
using Core.Loader.Sprite;
using Core.Pool;
using Core.Service;
using GameHotUpdate.Activity.Core;
using GameHotUpdate.Activity.UI.Common;
using GameHotUpdate.Config;
using GameHotUpdate.Item;
using GameHotUpdate.Item.UI;

namespace GameHotUpdate.Activity.UI.OrbitalDeparture
{
    using Task = System.Threading.Tasks.Task;
    
    /// <summary>
    /// 星旅启航活动UI
    /// </summary>
    public class OrbitalDepartureActivityUI : ActivityUIBehaviourBase
    {
        private ActivityBkComponent _activityBkComponent;
        private ActivityJoinComponent _activityJoinComponent;
        
        private ActivityDescritionComponent _activityDescritionComponent;
        private ActivityNameComponent _activityNameComponent;
        private ActivityTimeComponent _activityTimeComponent;
        
        private AwardPreviewComponent _awardPreviewComponent;
        
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
        }

        protected override async Task OnInit()
        {
            await base.OnInit();
            
            // 初始化组件
            _activityBkComponent.Init(this, activityInfo, ActivityData);
            _activityJoinComponent.Init(this, activityInfo, ActivityData);
            _activityDescritionComponent.Init(this, activityInfo, ActivityData);
            _activityNameComponent.Init(this, activityInfo, ActivityData);
            _activityTimeComponent.Init(this, activityInfo, ActivityData);
            _awardPreviewComponent.Init(this, activityInfo, ActivityData);
            
            // 初始化界面背景
            var backGround = await ServiceLocator.Get<ISpriteLoader>().LoadSpriteAsync(
                AbKeyCollection.Spriteatlas, ResKeyCollection.Atlas_Activity,
                activityInfo.f_bkUi_Res);
            // 设置界面背景
            _activityBkComponent.SetBackGround(backGround);
            
            UpdateBtnJoin();
            // 监听按钮
            _activityJoinComponent.OnClickJoin += OnTriggerJoin;
            
            _activityDescritionComponent.SetActivityDescrition(out var activityDescrition);
            activityDescrition.text = $"{activityInfo.f_description}";
            
            _activityNameComponent.SetTitle(out var txtActivityName);
            txtActivityName.text = $"{activityInfo.f_name}";
            
            _activityTimeComponent.SetDurationTime(out var txtTime);
            txtTime.text = $"{ToDurationStr(activityInfo.f_duration)}";
            
            // 解析奖励ID数组，获取物品格子
            ItemUtility.GetItemGrid(activityInfo.f_awardIds, grid =>
            {
                if (grid)
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

        private void OnTriggerJoin()
        {
            if (!ActivityData.IsComplete)
            {
                ActivityData.CurrentPro += 1;
                _activityJoinComponent.SetTitle(out var txtJoin);
                txtJoin.text = $"已领取";
            }
        }
        
        /// <summary>
        /// 更新按钮显示
        /// </summary>
        private void UpdateBtnJoin()
        {
            _activityJoinComponent.SetTitle(out var txtJoin);
            txtJoin.text = !ActivityData.IsComplete ? "立即领取" : "已领取";
        }
        
        /// <summary>
        /// 清理物品UI
        /// </summary>
        private void ClearItem()
        {
            foreach (var itemGrid in _itemGrids)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(itemGrid.gameObject);
            }
            _itemGrids.Clear();
        }
        
        protected override void OnHide()
        {
            ClearItem();
            _activityJoinComponent.OnClickJoin -= OnTriggerJoin;
        }
    }
}
