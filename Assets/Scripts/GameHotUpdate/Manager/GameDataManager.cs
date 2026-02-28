using System.Threading.Tasks;
using Core.Input.ActionAsset;
using Core.Input.CoreListen;
using Core.Log;
using Core.Music;
using Core.Serialize.Binary;
using Core.Serialize.Json;
using Core.Service;
using Core.Utility;
using Game.Activity;
using Game.Tasks;
using GameHotUpdate.Activity.Data;
using GameHotUpdate.Config;
using GameHotUpdate.Tasks;
using Newtonsoft.Json;

namespace GameHotUpdate.Manager
{
    /// <summary>
    /// 游戏数据管理器
    /// </summary>
    public class GameDataManager : IGameDataManager
    {
        private readonly IBinaryDataManager _binaryDataManager;
        private readonly IJsonManager _jsonManager;
        
        private readonly JsonSerializerSettings activitySettings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented,
        };

        public GameDataManager()
        {
            _binaryDataManager = ServiceLocator.Get<IBinaryDataManager>();
            _jsonManager = ServiceLocator.Get<IJsonManager>();
        }
        
        public async Task InitData()
        {
            _binaryDataManager.AddConfig(EConfigLoadType.Excel, async loader =>
            {
                await loader.LoadConfigAsync<RoleInfoContainer, RoleInfo>();
                await loader.LoadConfigAsync<MonsterInfoContainer, MonsterInfo>();
                await loader.LoadConfigAsync<SkillInfoContainer, SkillInfo>();
                await loader.LoadConfigAsync<StatusInfoContainer, StatusInfo>();
                await loader.LoadConfigAsync<DialogueInfoContainer, DialogueInfo>();
                await loader.LoadConfigAsync<BranchInfoContainer, BranchInfo>();
                await loader.LoadConfigAsync<TaskInfoContainer, TaskInfo>();
                await loader.LoadConfigAsync<TaskConditionInfoContainer, TaskConditionInfo>();
                await loader.LoadConfigAsync<NpcInfoContainer, NpcInfo>();
            
                await loader.LoadConfigAsync<ActivityInfoContainer,ActivityInfo>();
                await loader.LoadConfigAsync<ItemInfoContainer,ItemInfo>();
            });
            
            // 加载二进制配置
            await _binaryDataManager.LoadConfigAsync(AbKeyCollection.Gameconfig);
            LogManager.Log($"配置数据加载成功");
            
            // 读取本地音乐数据
            MusicData = await _binaryDataManager.LoadAsync<MusicData>(FileUtility.LocalMusicDataFileName);
            LogManager.Log($"本地音乐数据加载成功，{MusicData}");
            
            // 读取本地输入数据
            InputActionContainer = await _binaryDataManager.LoadAsync<MainActionMapDataContainer>(FileUtility.LocalInputDataFileName);
            LogManager.Log($"本地输入数据加载成功，{InputActionContainer}");
            
            // 读取任务数据
            TaskDataCollection = await _jsonManager.FromJsonAsync<TaskDataCollection>(PathUtility.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            LogManager.Log($"任务数据加载成功，{TaskDataCollection}");
            
            // 活动数据
            ActivityDataCollection = await _jsonManager.FromJsonAsync<ActivityDataCollection>(PathUtility.GetUserDataLocalSavePath(FileUtility.LocalActivityDataFileName),
                    settings: activitySettings);
            LogManager.Log($"活动数据加载成功，{ActivityDataCollection}");
        }

        public async Task SaveDataAsync()
        {
            // 保存任务数据
            await _jsonManager.SaveToJsonAsync(TaskDataCollection, PathUtility.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            LogManager.Log($"任务数据保存成功，{TaskDataCollection}");
            
            // 保存音乐数据
            await _binaryDataManager.SaveAsync(FileUtility.LocalMusicDataFileName, MusicData);
            LogManager.Log($"音乐数据保存成功，{MusicData}");
            
            // 保存输入数据
            await _binaryDataManager.SaveAsync(FileUtility.LocalInputDataFileName, InputActionContainer);
            LogManager.Log($"输入数据保存成功，{InputActionContainer}");
            
            // 活动数据
            await _jsonManager.SaveToJsonAsync(ActivityDataCollection,
                PathUtility.GetUserDataLocalSavePath(FileUtility.LocalActivityDataFileName),
                settings: activitySettings);
            LogManager.Log($"活动数据保存成功，{ActivityDataCollection}");
        }
        
        public ITaskDataCollection TaskDataCollection { get; private set; }
        
        public MusicData MusicData { get; private set; }

        public MainActionMapDataContainer InputActionContainer { get; private set; }
        
        public IActivityDataCollection ActivityDataCollection { get; private set; }
        
        public InputDataContainer InputDataContainer { get; private set; }
    }
}
