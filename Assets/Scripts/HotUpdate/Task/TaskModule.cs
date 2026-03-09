using Core.Serialize.Json;
using Core.Service;
using Core.UI;
using HotUpdate.Core.Manager;
using HotUpdate.Core.Module;
using HotUpdate.Core.Task;
using HotUpdate.Core.UI;
using HotUpdate.Core.UI.Helper;
using HotUpdate.Task.Core;
using HotUpdate.Task.Data;

namespace HotUpdate.Task
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 热更任务模块
    /// </summary>
    public class TaskModule : ITaskModule
    {
        public Task InitModuleAsync()
        {
            ServiceLocator.Register<ITaskManager>(TaskManager.Instance);
            // 初始化UIHelper
            ServiceLocator.Register<ITaskUiHelper>(new TaskUiHelper(ServiceLocator.Get<IUIManager>()));
            // 注册活动数据提供器
            ServiceLocator.Get<IGameManager>().GameDataManager.AddDataProvider(typeof(ITaskDataCollection), new TaskDataProvider(ServiceLocator.Get<IJsonManager>()));
            return Task.CompletedTask;
        }
    }
}
