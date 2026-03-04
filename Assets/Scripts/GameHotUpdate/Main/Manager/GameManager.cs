using System;
using Core.Log;
using Core.Quit;
using Core.Service;
using Core.Singleton;

namespace GameHotUpdate.Main.Manager
{
    using Task = System.Threading.Tasks.Task;
    
    /// <summary>
    /// 游戏管理器
    /// </summary>
    public class GameManager : SingletonBase<GameManager>, IGameManager
    {
        private readonly IQuitHandler _quitHandler = ServiceLocator.Get<IQuitHandler>();
        public GameDataManager GameDataManager { get; private set; }
        public GameServiceManger GameServiceManger { get; private set; }

        private GameManager()
        {
            _quitHandler.OnAppQuit += OnApplicationQuit;
        }

        public async Task Init()
        {
            GameDataManager = new GameDataManager();
            GameServiceManger = new GameServiceManger();
            
            GameServiceManger.InitService();
            await GameDataManager.InitData();
            
            try
            {

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
            return GameDataManager.SaveDataAsync();
        }
    }
}
