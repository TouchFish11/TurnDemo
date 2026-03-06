using System;
using Core.Log;
using Core.Quit;
using Core.Service;
using Core.Singleton;
using HotUpdate.Core;

namespace HotUpdate.Default
{
    using Task = System.Threading.Tasks.Task;
    
    /// <summary>
    /// 游戏管理器
    /// </summary>
    public class GameManager : SingletonBase<GameManager>, IGameManager
    {
        public override int Priority => -1;
        private IQuitHandler _quitHandler;
        private GameServiceManager gameServiceManager;
        public GameDataManager GameDataManager { get; private set; }
        
        public GameServiceManager GameServiceManager { get; private set; }

        private GameManager()
        {

        }

        public override Task InitAsync()
        {
            _quitHandler = ServiceLocator.Get<IQuitHandler>();
            _quitHandler.OnAppQuit += OnApplicationQuit;
            return Task.CompletedTask;
        }

        public async Task Init()
        {
            GameDataManager = new GameDataManager();
            GameServiceManager = new GameServiceManager();
            
            GameServiceManager.InitService();
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
