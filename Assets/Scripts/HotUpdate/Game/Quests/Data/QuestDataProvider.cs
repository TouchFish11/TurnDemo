using System.Threading.Tasks;
using Core.DI;
using Core.Log;
using Core.Serialize.Json;
using Core.Utility;
using HotUpdate.Base.Attributes;
using HotUpdate.Base.Collection;
using HotUpdate.Base.Data;

namespace HotUpdate.Game.Quests.Data
{
    /// <summary>
    /// 任务数据提供器
    /// </summary>
    [DataProviderId(typeof(IQuestDataProvider))]
    public class QuestDataProvider : IQuestDataProvider
    {
        [Inject] private IJsonManager _jsonManager;

        public IQuestCollection QuestCollection { get; private set; }
    
        
        public void LoadData()
        {
            
        }

        public void SaveData()
        {
            if (QuestCollection != null)
            {
                // 保存任务数据
                _jsonManager.SaveToJson(QuestCollection, PathUtility.GetUserDataLocalSavePath(FileSources.LocalTaskDataFileName));
                Logger.LogDebug(ELogTags.Quest, $"任务数据保存成功，{FileSources.LocalTaskDataFileName}");
            }
        }
        
        public async Task LoadDataAsync()
        {
            // 读取任务数据
            QuestCollection = await _jsonManager.FromJsonAsync<QuestCollection>(PathUtility.GetUserDataLocalSavePath(FileSources.LocalTaskDataFileName));
            Logger.LogDebug(ELogTags.Quest, $"QuestData loading successful");
        }

        public async Task SaveDataAsync()
        {
            if (QuestCollection != null)
            {
                // 保存任务数据
                await _jsonManager.SaveToJsonAsync(QuestCollection, PathUtility.GetUserDataLocalSavePath(FileSources.LocalTaskDataFileName));
                Logger.LogDebug(ELogTags.Quest, $"任务数据保存成功，{FileSources.LocalTaskDataFileName}");
            }
        }
    }
}
