using System;
using System.Threading.Tasks;
using Core.Log;
using Core.Quit;
using Core.Service;
using Core.Singleton;

namespace Game.Manager
{
    /// <summary>
    /// TODO：热更
    /// 游戏管理器
    /// </summary>
    public class GameManager : SingletonBase<GameManager>, IGameManager
    {
        public IGameDataManager GameDataManager { get; private set; }
        
        public IGameServiceManger GameServiceManger { get; private set; }

        private GameManager()
        {
            ServiceLocator.Get<IQuitHandler>().OnAppQuit += OnApplicationQuit;
        }

        public async Task Init(IGameDataManager gameDataManager, IGameServiceManger gameServiceManger)
        {
            GameDataManager = gameDataManager;
            GameServiceManger = gameServiceManger;
            
            try
            {
                gameServiceManger.InitService();
                await gameDataManager.InitData();
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
            return GameDataManager.SaveData();
        }
    }
}
