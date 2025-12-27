using System;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Framework
{
    /// <summary>
    /// 游戏数据管理器
    /// </summary>
    public class GameDataMgr : SingletonBase<GameDataMgr>
    {
        // 音乐数据
        private MusicData _musicData;
        // 输入系统动作数据容器对象
        private MainActionMapDataContainer _inputActionContainer;
        // 输入系统输入数据容器对象
        private InputDataContainer _inputDataContainer;
        // 任务数据集合
        private TaskDataCollection taskDataCollection;

        private GameDataMgr()
        {
            QuitHandler.Instance.OnAppQuit += OnApplicationQuit;
        }

        /// <summary>
        /// 异步初始化数据
        /// </summary>
        /// <param name="overCallBack"></param>
        /// <returns></returns>
        public async Task InitDataAsync()
        {
            try
            {
                // 读取表数据
                await BinaryDataMgr.Instance.LoadConfig();
                // 读取Json数据
                await JsonManager.Instance.LoadJsonAsync();
                // 读取音乐数据
                _musicData = BinaryDataMgr.Load<MusicData>(FileUtility.LocalMusicDataFileName);
                if (_musicData == null)
                {
                    LogManager.LogError($"初始化音乐数据失败");
                    return;
                }

                // 读取输入动作数据
                _inputActionContainer = BinaryDataMgr.Load<MainActionMapDataContainer>(FileUtility.LocalInputDataFileName);
                if (_inputActionContainer == null)
                {
                    LogManager.LogError($"初始化输入动作数据失败");
                    return;
                }

                // 读取任务数据
                taskDataCollection = await JsonManager.Instance.FromJsonAsync<TaskDataCollection>(PathManager.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            }
            catch (System.Exception ex)
            {
                LogManager.LogError($"初始化本地数据失败，{ex.Message}");
            }
        }

        private async Task OnApplicationQuit()
        {
            // 保存音乐数据
            if (MusicData != null)
            {
                BinaryDataMgr.Save(FileUtility.LocalMusicDataFileName, _musicData);
            }

            // 保存改键数据
            if (InputActionContainer != null)
            {
                BinaryDataMgr.Save(FileUtility.LocalInputDataFileName, _inputActionContainer);
            }

            // 保存任务数据
            if (taskDataCollection != null)
            {
                await JsonManager.Instance.ToJsonAsync(taskDataCollection, PathManager.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            }
        }

        /// <summary>
        /// 音乐数据
        /// </summary>
        public MusicData MusicData => _musicData;

        /// <summary>
        /// 输入系统动作数据容器
        /// </summary>
        public MainActionMapDataContainer InputActionContainer => _inputActionContainer;

        /// <summary>
        /// 输入系统输入数据容器
        /// </summary>
        public InputDataContainer InputDataContainer => _inputDataContainer;

        /// <summary>
        /// 任务数据集合
        /// </summary>
        public TaskDataCollection TaskDataCollection => taskDataCollection;
    }
}
