using Core.DI;
using HotUpdate.Base.Manager;
using HotUpdate.UI.Activity.Base;

namespace HotUpdate.UI.Activity.OrbitalDeparture
{
    public class OrbitalDepartureHandler : ActivityContentHandler<OrbitalDepartureActivityUI>
    {
        [Inject] private IActivityDataManager _activityDataManager;
        
        public void ReceiveReward()
        {
            if (!_activityDataManager.TryGetData(activity.ActivityId, out var activityData))
                return;
            
            if (!activityData.IsComplete)
            {
                activityData.CurrentPro += 1;
                activity.activityJoinComponent.SetTitle(out var txtJoin);
                txtJoin.text = $"已领取";
            }
        }

        public void UpdateShow()
        {
            if (!_activityDataManager.TryGetData(activity.ActivityId, out var activityData))
                return;
            
            activity.activityJoinComponent.SetTitle(out var txtJoin);
            txtJoin.text = !activityData.IsComplete ? "立即领取" : "已领取";
        }
    }
}
