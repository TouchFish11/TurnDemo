using System.Threading.Tasks;
using Core.Config;
using Core.Loader;
using Core.Log;
using Core.Reflection;
using Core.Service;
using GameHotUpdate.Activity.Core;
using GameHotUpdate.Activity.UI.Common;

namespace GameHotUpdate.Activity.UI.OrbitalDeparture
{
    /// <summary>
    /// 星旅启航活动UI
    /// </summary>
    public class OrbitalDepartureActivityUI : ActivityBase
    {
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
            
            _awardPreviewComponent.SetAwards();
        }

        private void OnTriggerJoin()
        {
            LogManager.Log($"参与按钮点击");
        }

        private void OnTriggerLimitTimeAward()
        {
            LogManager.Log($"限时奖励按钮点击");
        }
        
        protected override void OnHide()
        {
            _activityJoinComponent.OnClickJoin -= OnTriggerJoin;
            _limitTimeAwardComponent.OnClickAward -= OnTriggerLimitTimeAward;
        }
    }
}
