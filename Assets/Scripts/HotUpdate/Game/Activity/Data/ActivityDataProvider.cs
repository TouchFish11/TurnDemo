using System.Threading.Tasks;
using Core.DI;
using Core.Log;
using Core.Serialize.Json;
using Core.Utility;
using HotUpdate.Base.Activity;
using Newtonsoft.Json;

namespace HotUpdate.Game.Activity.Data
{
    /// <summary>
    /// 活动数据提供器
    /// </summary>
    public class ActivityDataProvider : IActivityDataProvider
    {
        // Newtonsoft设置配置
        private readonly JsonSerializerSettings activityDataSettings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented,
        };
        
        /// <summary>
        /// 活动数据集合
        /// </summary>
        public IActivityDataCollection ActivityDataCollection { get; private set; }

        public async Task LoadDataAsync()
        {
            // 活动数据
            ActivityDataCollection = await DIContainer.GetInstance<IJsonManager>()
                .FromJsonAsync<ActivityDataCollection>(
                    PathUtility.GetUserDataLocalSavePath(FileUtility.LocalActivityDataFileName),
                    settings: activityDataSettings);
            
            Logger.Log($"活动数据加载成功，{ActivityDataCollection}");
        }

        public async Task SaveDataAsync()
        {
            // 活动数据
            await DIContainer.GetInstance<IJsonManager>().SaveToJsonAsync(ActivityDataCollection,
                PathUtility.GetUserDataLocalSavePath(FileUtility.LocalActivityDataFileName),
                settings: activityDataSettings);
            Logger.Log($"{nameof(ActivityDataProvider)}.{nameof(SaveDataAsync)}:活动数据保存成功，{FileUtility.LocalActivityDataFileName}");
        }

        public void SaveData()
        {
            // 活动数据
            DIContainer.GetInstance<IJsonManager>().SaveToJson(ActivityDataCollection,
                PathUtility.GetUserDataLocalSavePath(FileUtility.LocalActivityDataFileName),
                settings: activityDataSettings);
            Logger.Log($"{nameof(ActivityDataProvider)}.{nameof(SaveData)}:活动数据保存成功，{FileUtility.LocalActivityDataFileName}");
        }

        public IActivityDataCollection GetData()
        {
            return ActivityDataCollection;
        }
    }
}
