using System.Threading.Tasks;

namespace Framework
{
    /// <summary>
    /// 游戏数据管理器
    /// </summary>
    public class GameDataMgr : SingletonBase<GameDataMgr>
    {
        //音乐数据
        private MusicData _musicData;
        //输入系统动作数据容器对象
        private InputActionContainer _inputActionContainer;
        //输入系统输入数据容器对象
        private InputDataContainer _inputDataContainer;

        private GameDataMgr() { }

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
                await BinaryDataMgr.Instance.InitTableAsync();
                // 读取Json数据
                await JsonManager.Instance.LoadJsonAsync();
                // 读取音乐数据
                _musicData = BinaryDataMgr.Instance.Load<MusicData>(FileUtility.LocalMusicDataFileName);
                if (_musicData == null)
                {
                    LogManager.LogError($"初始化音乐数据失败");
                    return;
                }

                // 读取输入动作数据
                _inputActionContainer = BinaryDataMgr.Instance.Load<InputActionContainer>(FileUtility.LocalInputDataFileName);
                if (_inputActionContainer == null)
                {
                    LogManager.LogError($"初始化输入动作数据失败");
                    return;
                }
            }
            catch (System.Exception ex)
            {
                LogManager.LogError($"初始化本地数据失败，{ex.Message}");
            }
        }

        /// <summary>
        /// 音乐数据
        /// </summary>
        public MusicData MusicData { get => _musicData; }

        /// <summary>
        /// 输入系统动作数据容器
        /// </summary>
        public InputActionContainer InputActionContainer { get => _inputActionContainer; }

        /// <summary>
        /// 输入系统输入数据容器
        /// </summary>
        public InputDataContainer InputDataContainer { get => _inputDataContainer; }

    }
}
