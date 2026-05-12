using Core.Serialize.Json;
using Core.Utility;
using HotUpdate.Base.Quest;

namespace HotUpdate.Game.Quests.Data
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 任务数据提供器
    /// </summary>
    public class TaskDataProvider : ITaskDataProvider
    {
        private readonly IJsonManager _jsonManager;

        public IQuestCollection QuestCollection { get; private set; }

        public TaskDataProvider(IJsonManager jsonManager)
        {
            _jsonManager = jsonManager;
        }

        public async Task LoadDataAsync()
        {
            // 读取任务数据
            QuestCollection = await _jsonManager.FromJsonAsync<QuestCollection>(PathUtility.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            LogManager.Log($"任务数据加载成功，{QuestCollection}");
        }
        
        public async Task SaveDataAsync()
        {
            // 保存任务数据
            await _jsonManager.SaveToJsonAsync(QuestCollection, PathUtility.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            LogManager.Log($"{nameof(TaskDataProvider)}.{nameof(SaveDataAsync)}:任务数据保存成功，{FileUtility.LocalTaskDataFileName}");
        }

        public void SaveData()
        {
            // 保存任务数据
            _jsonManager.SaveToJson(QuestCollection, PathUtility.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            LogManager.Log($"{nameof(TaskDataProvider)}.{nameof(SaveData)}:任务数据保存成功，{FileUtility.LocalTaskDataFileName}");
        }
    }
}
