using System;
using System.Threading.Tasks;
using Core.DataPersistence.Binary;
using Core.DataPersistence.Json;
using Core.InputSystem.ActionAsset;
using Core.InputSystem.CoreListen;
using Core.Log;
using Core.Music;
using Core.Service;
using Core.Singleton;
using Core.Utility;

namespace Core.DataPersistence
{
    /// <summary>
    /// 游戏数据管理器
    /// </summary>
    public class GameDataManager : SingletonBase<GameDataManager>, IGameDataManager
    {
        public event Func<Task> OnInitData;
        
        public event Func<Task> OnSaveData; 
        
        private GameDataManager()
        {
            QuitHandler.QuitHandler.Instance.OnAppQuit += OnApplicationQuit;
        }

        public async Task InitDataAsync()
        {
            try
            {
                // 加载二进制配置
                await ServiceLocator.Get<IBinaryDataManager>().LoadConfig();
                // 加载Json配置
                await JsonManager.Instance.LoadJsonAsync();
                // 读取本地音乐数据
                MusicData = ServiceLocator.Get<IBinaryDataManager>().Load<MusicData>(FileUtility.LocalMusicDataFileName);
                if (MusicData == null)
                {
                    LogManager.LogError($"音乐数据读取失败");
                    return;
                }

                // 读取本地输入数据
                InputActionContainer = ServiceLocator.Get<IBinaryDataManager>().Load<MainActionMapDataContainer>(FileUtility.LocalInputDataFileName);
                if (InputActionContainer == null)
                {
                    LogManager.LogError($"输入数据读取失败");
                    return;
                }
                
                // 初始化自定义数据
                await OnInitData?.Invoke();
            }
            catch (Exception ex)
            {
                LogManager.LogError($"数据初始化失败:{ex.Message}");
            }
        }

        /// <summary>
        /// 应用退出事件回调
        /// </summary>
        /// <returns></returns>
        private Task OnApplicationQuit()
        {
            // 保存音乐数据
            if (MusicData != null)
            {
                ServiceLocator.Get<IBinaryDataManager>().Save(FileUtility.LocalMusicDataFileName, MusicData);
            }

            // 保存输入数据
            if (InputActionContainer != null)
            {
                ServiceLocator.Get<IBinaryDataManager>().Save(FileUtility.LocalInputDataFileName, InputActionContainer);
            }

            return OnSaveData?.Invoke();
        }

        public MusicData MusicData { get; private set; }

        public MainActionMapDataContainer InputActionContainer { get; private set; }
        
        public InputDataContainer InputDataContainer { get; private set; }
    }
}
