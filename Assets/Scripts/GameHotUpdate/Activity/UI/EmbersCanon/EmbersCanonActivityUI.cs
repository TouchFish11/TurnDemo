using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Config;
using Core.Loader;
using Core.Log;
using Core.Pool;
using Core.Reflection;
using Core.Service;
using GameHotUpdate.Activity.Core;
using GameHotUpdate.Activity.UI.Common;
using GameHotUpdate.Item;
using GameHotUpdate.Item.UI;

namespace GameHotUpdate.Activity.UI.EmbersCanon
{
    /// <summary>
    /// 余烬圣典活动UI
    /// </summary>
    public class EmbersCanonActivityUI : ActivityBase
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
            _activityBkComponent = this.GetComponentInChildren<ActivityBkComponent>();
            _activityJoinComponent = this.GetComponentInChildren<ActivityJoinComponent>();
            _activityDescritionComponent = this.GetComponentInChildren<ActivityDescritionComponent>();
            _activityNameComponent = this.GetComponentInChildren<ActivityNameComponent>();
            _activityTimeComponent = this.GetComponentInChildren<ActivityTimeComponent>();
            _awardPreviewComponent = this.GetComponentInChildren<AwardPreviewComponent>();
            _limitTimeAwardComponent = this.GetComponentInChildren<LimitTimeAwardComponent>();
        }
        
        protected override async Task OnInit()
        {
            // 初始化界面背景
            var backGround = await ServiceLocator.Get<IFactoryManager>().GetFactory<IAssetLoaderFactory, AssetLoaderFactory>().GetSpriteLoader()
                .GetSpriteAsync(ResKeyCollection.Atlas_Activity,
                    this.activityInfo.f_bkUi_Res);
            _activityBkComponent.SetBackGround(backGround);

            _activityJoinComponent.OnClickJoin += OnTriggerJoin;
            _limitTimeAwardComponent.OnClickAward += OnTriggerLimitTimeAward;
            
            _activityDescritionComponent.SetActivityDescrition(out var activityDescrition);
            activityDescrition.text = $"{activityInfo.f_description}";
            
            _activityNameComponent.SetTitle(out var txtActivityName);
            txtActivityName.text = $"{activityInfo.f_name}";
            
            _activityTimeComponent.SetDurationTime(out var txtTime);
            txtTime.text = $"{activityInfo}";
            
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
        
        private void OnTriggerJoin()
        {
            LogManager.Log($"参与按钮点击");
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
                ServiceLocator.Get<IPoolManager>().PushObj(itemGrid.gameObject);
            }
            _itemGrids.Clear();
        }
        
        protected override void OnHide()
        {
            ClearItem();
            
            _activityJoinComponent.OnClickJoin -= OnTriggerJoin;
            _limitTimeAwardComponent.OnClickAward -= OnTriggerLimitTimeAward;
        }
    }
}
