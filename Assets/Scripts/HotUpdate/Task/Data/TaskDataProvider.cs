using Core.Log;
using Core.Serialize.Json;
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
        private readonly IJsonManager _jsonManager;
        
        /// <summary>
        /// 任务数据集合
        /// </summary>
        public ITaskDataCollection TaskDataCollection { get; private set; }

        public TaskDataProvider(IJsonManager jsonManager)
        {
            _jsonManager = jsonManager;
        }

        public async Task LoadDataAsync()
        {
            // 读取任务数据
            TaskDataCollection = await _jsonManager.FromJsonAsync<TaskDataCollection>(PathUtility.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            LogManager.Log($"任务数据加载成功，{TaskDataCollection}");
        }
        
        public async Task SaveDataAsync()
        {
            // 保存任务数据
            await _jsonManager.SaveToJsonAsync(TaskDataCollection, PathUtility.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            LogManager.Log($"{nameof(TaskDataProvider)}.{nameof(SaveDataAsync)}:任务数据保存成功，{FileUtility.LocalTaskDataFileName}");
        }

        public void SaveData()
        {
            // 保存任务数据
            _jsonManager.SaveToJson(TaskDataCollection, PathUtility.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            LogManager.Log($"{nameof(TaskDataProvider)}.{nameof(SaveData)}:任务数据保存成功，{FileUtility.LocalTaskDataFileName}");
        }

        public ITaskDataCollection GetData()
        {
            return TaskDataCollection;
        }
    }
}
