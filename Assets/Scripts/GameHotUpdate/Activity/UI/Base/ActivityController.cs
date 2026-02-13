using Core.AssetBundles.Management;
using Core.Config;
using Core.DataPersistence.Binary;
using Core.Loader.Sprite;
using Core.Loader.UI;
using Core.Log;
using Core.Pool;
using Core.Reflection;
using Core.Service;
using Core.UI;
using Core.UI.MVC;
using Game.Manager;
using Game.Objects;
using GameHotUpdate.Activity.Core;
using GameHotUpdate.Activity.Data;
using GameHotUpdate.Activity.UI.EmbersCanon;

namespace GameHotUpdate.Activity.UI.Base
{
    /// <summary>
    /// 活动界面控制器
    /// </summary>
    public class ActivityController : UIController<ActivityView, ActivityModel>
    {
        protected override async System.Threading.Tasks.Task OnInit()
        {
            // 读取活动数据
            var infoDic = ServiceLocator.Get<IBinaryDataManager>().GetConfig<ActivityInfoContainer>(EConfigLoadType.Excel).dataDic;
            // 创建UI
            foreach (var activityInfo in infoDic.Values)
            {
                var activityUI = await ServiceLocator.Get<IUiLoader>().GetUIObject<ActivityUI>(EAssetBundleType.UI, ResKeyCollection.ActivityUI, view.SvActivityContent);
                // 加载图标
                var icon = await ServiceLocator.Get<ISpriteLoader>().LoadSpriteAsync(
                    ResKeyCollection.Atlas_Activity,
                        activityInfo.f_bkUi_Res);
                // 初始化UI
                activityUI.Init(icon, activityInfo, view.ActivityGroup, this);
                // 缓存UI
                model.CacheActivity(activityUI);
            }
            
            // 默认选中第一个UI
            model.GetFirstActivityUI().SelectActivity();
        }

        protected override void ButtonOnClick(string btnName)
        {
            switch (btnName)
            {
                case nameof(view.btnClose):
                    ServiceLocator.Get<IUIManager>().DestroyView(this);
                    break;
            }
        }

        public async System.Threading.Tasks.Task UpdateDetailActivity(ActivityInfo activityInfo)
        {
            if (model.Activity != null && activityInfo.f_id == model.Activity.ActivityData.ActivityId)
            {
                return;
            }
            
            // 通过配置信息获取对应类型
            var activityType = ServiceLocator.Get<IFactoryManager>().GetFactory<IActivityFactory, ActivityFactory>()
                .GetActivity(activityInfo.f_detailUI_res);
            
            // 活动本地活动数据
            var activityDataCollection = ServiceLocator.Get<IGameManager>().GameDataManager.ActivityDataCollection as ActivityDataCollection;
            
            // 获取活动UI对象
            var activity = await ServiceLocator.Get<IUiLoader>().GetUIObject<IActivity>(EAssetBundleType.UI, activityInfo.f_detailUI_res,
                view.ActivityDetailArea);
            // 初始化详细界面
            if (!activityDataCollection.TryGetValue(activityInfo.f_id, out var activityData))
            {
                // 新增活动数据
                // TODO：暂时这样处理
                activityData = activityInfo.f_id switch
                {
                    1001 => new ActivityData { ActivityId = activityInfo.f_id },
                    1002 => new EmbersCanonData { ActivityId = activityInfo.f_id },
                    _ => activityData
                };
                
                // 缓存
                activityDataCollection.TryAdd(activityInfo.f_id, activityData);
            }

            activity?.Init(activityData, activityInfo);
            // 缓存界面
            model.UpdateActivityDetailUI(activityType, activity);
        }

        public override void Destroy()
        {
            // 清除缓存
            ServiceLocator.Get<IPoolManager>().ClearTypes(typeof(ActivityUI));
            foreach (var activityType in model.GetActivityTypes())
            {
                ServiceLocator.Get<IPoolManager>().ClearTypes(activityType);
            }
            base.Destroy();
        }
    }
}
