using System.Threading.Tasks;
using Core.Log;
using Core.Serialize.Json;
using Core.Service;
using Core.Utility;
using HotUpdate.Core.Provider;
using HotUpdate.Core.Task;

namespace HotUpdate.Task.Data
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 任务数据提供器
    /// </summary>
    public class TaskDataProvider : IDataProvider<ITaskDataCollection>
    {
        /// <summary>
        /// 任务数据集合
        /// </summary>
        public ITaskDataCollection TaskDataCollection { get; private set; }
        
        public async Task<ITaskDataCollection> GetDataAsync()
        {
            // 读取任务数据
            TaskDataCollection = await ServiceLocator.Get<IJsonManager>().FromJsonAsync<TaskDataCollection>(PathUtility.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            LogManager.Log($"任务数据加载成功，{TaskDataCollection}");
            return TaskDataCollection;
        }
        
        public async Task SaveDataAsync()
        {
            // 保存任务数据
            await ServiceLocator.Get<IJsonManager>().SaveToJsonAsync(TaskDataCollection, PathUtility.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            LogManager.Log($"任务数据保存成功，{TaskDataCollection}");
        }
    }
}
