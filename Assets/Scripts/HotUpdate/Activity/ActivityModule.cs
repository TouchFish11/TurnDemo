using System.Threading.Tasks;
using Core.Service;
using Core.UI;
using HotUpdate.Activity.Core;
using HotUpdate.Activity.Data;
using HotUpdate.Core.Activity;
using HotUpdate.Core.Manager;
using HotUpdate.Core.Module;
using HotUpdate.Core.UI.Helper;

namespace HotUpdate.Activity
{
    /// <summary>
    /// 活动模块
    /// </summary>
    public class ActivityModule : IActivityModule
    {
        public Task InitModuleAsync()
        {
            // 注册活动数据提供器
            ServiceLocator.Get<IGameManager>().GameDataManager.AddDataProvider(typeof(IActivityDataCollection), new ActivityDataProvider());

            ServiceLocator.Register<IActivityUiHelper>(new ActivityUiHelper(ServiceLocator.Get<IUIManager>()));
            
            return Task.CompletedTask; 
        }
    }
}
