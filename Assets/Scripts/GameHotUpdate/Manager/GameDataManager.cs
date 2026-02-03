using System.Threading.Tasks;
using Core.DataPersistence.Binary;
using Core.DataPersistence.Json;
using Core.Input.ActionAsset;
using Core.Input.CoreListen;
using Core.Log;
using Core.Music;
using Core.Service;
using Core.Utility;
using Game.Manager;
using Game.Tasks;
using GameHotUpdate.Tasks;

namespace GameHotUpdate.Manager
{
    /// <summary>
    /// 游戏数据管理器
    /// </summary>
    public class GameDataManager : IGameDataManager
    {
        public async Task InitData()
        {
            // 加载二进制配置
            await ServiceLocator.Get<IBinaryDataManager>().LoadConfig();
            LogManager.Log($"配置数据加载成功");
            
            // 读取本地音乐数据
            MusicData = ServiceLocator.Get<IBinaryDataManager>().Load<MusicData>(FileUtility.LocalMusicDataFileName);
            LogManager.Log($"本地音乐数据加载成功，{MusicData}");
            
            // 读取本地输入数据
            InputActionContainer = ServiceLocator.Get<IBinaryDataManager>().Load<MainActionMapDataContainer>(FileUtility.LocalInputDataFileName);
            LogManager.Log($"本地输入数据加载成功，{InputActionContainer}");
            
            // 读取任务数据
            TaskDataCollection = await ServiceLocator.Get<IJsonManager>().FromJsonAsync<TaskDataCollection>(PathUtility.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            LogManager.Log($"任务数据加载成功，{TaskDataCollection}");
            
            // 活动数据
            // ...
        }

        public async Task SaveData()
        {
            // 保存任务数据
            await ServiceLocator.Get<IJsonManager>().SaveToJsonAsync(TaskDataCollection, PathUtility.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            LogManager.Log($"任务数据保存成功，{TaskDataCollection}");
            
            // 保存音乐数据
            ServiceLocator.Get<IBinaryDataManager>().Save(FileUtility.LocalMusicDataFileName, MusicData);
            LogManager.Log($"音乐数据保存成功，{MusicData}");
            
            // 保存输入数据
            ServiceLocator.Get<IBinaryDataManager>().Save(FileUtility.LocalInputDataFileName, InputActionContainer);
            LogManager.Log($"输入数据保存成功，{InputActionContainer}");
            
            // 活动数据
            // ...
        }
        
        public ITaskDataCollection TaskDataCollection { get; private set; }
        
        public MusicData MusicData { get; private set; }

        public MainActionMapDataContainer InputActionContainer { get; private set; }
        
        public InputDataContainer InputDataContainer { get; private set; }
    }
}
