using System.Threading.Tasks;
using Core.Service;
using HotUpdate.Activity.Data;
using HotUpdate.Core.Activity;
using HotUpdate.Core.Manager;
using HotUpdate.Core.Module;

namespace HotUpdate.Activity
{
    /// <summary>
    /// 活动模块
    /// </summary>
    public class ActivityModule : IModule
    {
        public Task InitModuleAsync()
        {
            // 注册活动数据提供器
            ServiceLocator.Get<IGameManager>().GameDataManager.AddDataProvider(typeof(IActivityDataCollection), new ActivityDataProvider());
            return Task.CompletedTask; 
        }
    }
}
