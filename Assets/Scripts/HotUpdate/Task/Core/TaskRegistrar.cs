using Core.Service;
using HotUpdate.Core.Manager;
using HotUpdate.Core.Task;

namespace HotUpdate.Task.Core
{
    /// <summary>
    /// 游戏任务模块注册器
    /// </summary>
    public class TaskRegistrar : IGameServiceRegistrar
    {
        public void RegisterServices()
        {
            ServiceLocator.Register<ITaskManager>(TaskManager.Instance);
        }
    }
}
