using Core.Service;
using HotUpdate.Core.Manager;
using HotUpdate.Core.Module;
using HotUpdate.Core.Task;
using HotUpdate.Task.Data;

namespace HotUpdate.Task
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 任务模块
    /// </summary>
    public class TaskModule : IModule
    {
        public Task InitModuleAsync()
        {
            // 注册活动数据提供器
            ServiceLocator.Get<IGameManager>().GameDataManager.AddDataProvider(typeof(ITaskDataCollection), new TaskDataProvider());
            return Task.CompletedTask;
        }
    }
}
