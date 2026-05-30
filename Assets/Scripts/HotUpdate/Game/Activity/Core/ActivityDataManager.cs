using System.Threading.Tasks;
using Core.DI;
using Core.Log;
using Core.Serialize.Json;
using Core.Utility;
using HotUpdate.Base.Attributes;
using HotUpdate.Base.Collection;
using HotUpdate.Base.Data;
using HotUpdate.Base.Manager;

namespace HotUpdate.Game.Activity.Core
{
    /// <summary>
    /// 活动数据管理器
    /// </summary>
    [DataManagerId(typeof(IActivityDataManager))]
    public class ActivityDataManager : IActivityDataManager
    {
        [Inject] private IJsonManager _jsonManager;
        
        /// <summary>
        /// 活动数据集合
        /// </summary>
        public IActivityDataCollection ActivityDataCollection { get; private set; }

        public bool TryGetData(int activityId, out ActivityData activityData)
        {
            return ((ActivityDataCollection)ActivityDataCollection).TryGetValue(activityId, out activityData);
        }

        public async Task LoadDataAsync()
        {
            // 加载活动数据
            ActivityDataCollection = await _jsonManager.FromJsonAsync<ActivityDataCollection>(
                    PathUtility.GetUserDataLocalSavePath(FileUtility.LocalActivityDataFileName),
                    settings: NewtonsoftJsonUtility.SerializerSettings);
            
            Logger.Log($"[{nameof(ActivityDataManager)}]: 活动数据 {FileUtility.LocalActivityDataFileName} 加载成功");
        }

        public void SaveData()
        {
            // 活动数据
            _jsonManager.SaveToJson(ActivityDataCollection,
                PathUtility.GetUserDataLocalSavePath(FileUtility.LocalActivityDataFileName),
                settings: NewtonsoftJsonUtility.SerializerSettings);
            Logger.Log($"[{nameof(ActivityDataManager)}] :活动数据 {FileUtility.LocalActivityDataFileName} 保存成功");
        }

        public async Task SaveDataAsync()
        {
            // 保存活动数据
            await _jsonManager.SaveToJsonAsync(ActivityDataCollection,
                PathUtility.GetUserDataLocalSavePath(FileUtility.LocalActivityDataFileName),
                settings: NewtonsoftJsonUtility.SerializerSettings);
            Logger.Log($"[{nameof(ActivityDataManager)}]: 活动数据 {FileUtility.LocalActivityDataFileName} 保存成功");
        }
    }
}
