using System.Threading.Tasks;
using Core.DI;
using Core.Log;
using Core.Serialize.Json;
using Core.Utility;
using HotUpdate.Base.Collection;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Quest;

namespace HotUpdate.Game.Quests.Data
{
    public class QuestDataManager : IQuestDataManager
    {
        [Inject] private IJsonManager _jsonManager;

        public IQuestCollection QuestCollection { get; private set; }
    
        public async Task LoadDataAsync()
        {
            // 读取任务数据
            QuestCollection = await _jsonManager.FromJsonAsync<QuestCollection>(PathUtility.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            Logger.Log($"任务数据加载成功，{QuestCollection}");
        }

        public void SaveData()
        {
            // 保存任务数据
            _jsonManager.SaveToJson(QuestCollection, PathUtility.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            Logger.Log($"{nameof(QuestDataManager)}.{nameof(SaveData)}:任务数据保存成功，{FileUtility.LocalTaskDataFileName}");
        }

        public async Task SaveDataAsync()
        {
            // 保存任务数据
            await _jsonManager.SaveToJsonAsync(QuestCollection, PathUtility.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            Logger.Log($"{nameof(QuestDataManager)}.{nameof(SaveDataAsync)}:任务数据保存成功，{FileUtility.LocalTaskDataFileName}");
        }
    }
}
