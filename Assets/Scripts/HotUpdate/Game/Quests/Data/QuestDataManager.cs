using System.Threading.Tasks;
using Core.DI;
using Core.Log;
using Core.Serialize.Json;
using Core.Utility;
using HotUpdate.Base.Attributes;
using HotUpdate.Base.Collection;
using HotUpdate.Base.Manager;

namespace HotUpdate.Game.Quests.Data
{
    /// <summary>
    /// 任务管理器
    /// </summary>
    [DataManagerId(typeof(IQuestDataManager))]
    public class QuestDataManager : IQuestDataManager
    {
        [Inject] private IJsonManager _jsonManager;

        public IQuestCollection QuestCollection { get; private set; }
    
        public async Task LoadDataAsync()
        {
            // 读取任务数据
            QuestCollection = await _jsonManager.FromJsonAsync<QuestCollection>(PathUtility.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            Logger.Log($"[{nameof(QuestDataManager)}]: QuestData loading successful");
        }

        public void SaveData()
        {
            if (QuestCollection != null)
            {
                // 保存任务数据
                _jsonManager.SaveToJson(QuestCollection, PathUtility.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
                Logger.Log($"{nameof(QuestDataManager)}: 任务数据保存成功，{FileUtility.LocalTaskDataFileName}");
            }
        }

        public async Task SaveDataAsync()
        {
            if (QuestCollection != null)
            {
                // 保存任务数据
                await _jsonManager.SaveToJsonAsync(QuestCollection, PathUtility.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
                Logger.Log($"{nameof(QuestDataManager)}: 任务数据保存成功，{FileUtility.LocalTaskDataFileName}");
            }
        }
    }
}
