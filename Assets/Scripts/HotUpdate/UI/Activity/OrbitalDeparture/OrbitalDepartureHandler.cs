using Core.DI;
using Core.Serialize.Binary;
using HotUpdate.Base.Data;
using HotUpdate.UI.Activity.Base;

namespace HotUpdate.UI.Activity.OrbitalDeparture
{
    public class OrbitalDepartureHandler : ActivityContentHandler<OrbitalDepartureActivityUI>
    {
        [Inject] private IActivityDataProvider activityDataProvider;
        [Inject] private IBinaryDataManager _binaryDataManager;
        
        public void ReceiveReward()
        {
            if (!activityDataProvider.TryGetData(activity.ActivityId, out var activityData))
                return;
            
            if (!activityData.IsComplete)
            {
                var activityInfo = _binaryDataManager.GetConfig<ActivityInfoContainer>(EConfigLoadType.Excel).dataDic[activity.ActivityId];
                if (activityInfo.f_maxPro > activityData.CurrentPro)
                {
                    activityData.CurrentPro += 1;
                    activityData.IsComplete = activityInfo.f_maxPro == activityData.CurrentPro;
                }
                activity.activityJoinComponent.SetTitle(out var txtJoin);
                txtJoin.text = $"已领取";
            }
        }

        public void UpdateShow()
        {
            if (!activityDataProvider.TryGetData(activity.ActivityId, out var activityData))
                return;
            
            activity.activityJoinComponent.SetTitle(out var txtJoin);
            txtJoin.text = !activityData.IsComplete ? "立即领取" : "已领取";
        }
    }
}
