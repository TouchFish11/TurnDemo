using System.Threading.Tasks;
using Core.Log;
using Core.Serialize.Json;
using Core.Service;
using Core.Utility;
using HotUpdate.Core.Activity;
using HotUpdate.Core.Provider;
using Newtonsoft.Json;

namespace HotUpdate.Activity.Data
{
    /// <summary>
    /// 活动数据提供器
    /// </summary>
    public class ActivityDataProvider : IDataProvider<IActivityDataCollection>
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
            ActivityDataCollection = await ServiceLocator.Get<IJsonManager>()
                .FromJsonAsync<ActivityDataCollection>(
                    PathUtility.GetUserDataLocalSavePath(FileUtility.LocalActivityDataFileName),
                    settings: activityDataSettings);
            
            LogManager.Log($"活动数据加载成功，{ActivityDataCollection}");
        }

        public async Task SaveDataAsync()
        {
            // 活动数据
            await ServiceLocator.Get<IJsonManager>().SaveToJsonAsync(ActivityDataCollection,
                PathUtility.GetUserDataLocalSavePath(FileUtility.LocalActivityDataFileName),
                settings: activityDataSettings);
            LogManager.Log($"活动数据保存成功，{ActivityDataCollection}");
        }
        
        public IActivityDataCollection GetData()
        {
            return ActivityDataCollection;
        }
    }
}
