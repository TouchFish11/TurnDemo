using Core.Loader.Object;
using Core.Loader.Sprite;
using Core.Reflection;
using Core.Serialize.Binary;
using Core.Service;
using Core.UI.MVC;
using HotUpdate.Activity.Core;
using HotUpdate.Activity.Data;
using HotUpdate.Config;
using HotUpdate.Main.Manager;
using HotUpdate.Main.UI;

namespace HotUpdate.Activity.UI.Base
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 活动界面控制器
    /// </summary>
    public class ActivityController : UIController<ActivityView, ActivityModel>
    {
        private readonly IGameManager _gameManager = ServiceLocator.Get<IGameManager>();
        
        protected override async Task OnShow()
        {
            // 读取活动数据
            var infoDic = ServiceLocator.Get<IBinaryDataManager>().GetConfig<ActivityInfoContainer>(EConfigLoadType.Excel).dataDic;
            // 创建UI
            foreach (var activityInfo in infoDic.Values)
            {
                var activityUI = await ServiceLocator.Get<IPrefabLoader>().GetObjectAsync<ActivityUI>(AbKeyCollection.Ui, ResKeyCollection.ActivityUI, view.SvActivityContent);
                // 加载图标
                var icon = await ServiceLocator.Get<ISpriteLoader>().LoadSpriteAsync(AbKeyCollection.Spriteatlas, ResKeyCollection.Atlas_Activity,
                    activityInfo.f_bkUi_Res);
                // 初始化UI
                activityUI.Init(icon, activityInfo, view.ActivityGroup, this);
                // 缓存UI
                model.CacheActivity(activityUI);
            }
            
            // 默认选中第一个UI
            model.GetFirstActivityUI().SelectActivity();
        }
        
        protected override Task OnInit()
        {
            return Task.CompletedTask;
        }
        
        protected override Task OnHide()
        {
            // 显示主界面
            return uiManager.SetViewActive(uiManager.GetController<MainController>(), true);
        }

        protected override void ButtonOnClick(string btnName)
        {
            switch (btnName)
            {
                case nameof(view.btnClose):
                    uiManager.DestroyView(AbKeyCollection.Ui, this);
                    break;
            }
        }

        public async Task UpdateDetailActivity(ActivityInfo activityInfo)
        {
            if (model.Activity != null && activityInfo.f_id == model.Activity.ActivityData.ActivityId)
            {
                return;
            }
            
            // 活动本地活动数据
            var activityDataCollection = _gameManager.GameDataManager.ActivityDataCollection as ActivityDataCollection;
            // 获取活动UI对象
            var activity = await prefabLoader.GetObjectAsync<ActivityUIBehaviourBase>(AbKeyCollection.Ui, activityInfo.f_detailUI_res,
                view.ActivityDetailArea);
            // 初始化详细界面
            if (!activityDataCollection.TryGetValue(activityInfo.f_id, out var activityData))
            {
                // 新增活动数据
                activityData = ServiceLocator.Get<IFactoryManager>()
                    .GetFactory<IActivityDataFactory, ActivityDataFactory>()
                    .GetData(activityInfo.f_id);
                // 初始化ID
                activityData.ActivityId = activityInfo.f_id;
                // 缓存
                activityDataCollection.TryAdd(activityInfo.f_id, activityData);
            }

            activity?.Init(activityData, activityInfo);
            // 缓存界面
            model.UpdateActivityDetailUI(activity);
        }
    }
}
