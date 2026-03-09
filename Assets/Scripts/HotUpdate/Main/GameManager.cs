using System;
using Core.Log;
using Core.Mono;
using Core.Service;
using Core.Singleton;
using HotUpdate.Core.Manager;

namespace HotUpdate.Main
{
    using Task = System.Threading.Tasks.Task;
    
    /// <summary>
    /// 游戏管理器
    /// </summary>
    public class GameManager : IInitializable, IGameManager
    {
        public int Priority => -1;
        public GameDataManager GameDataManager { get; private set; }

        public Task InitAsync()
        {
            ServiceLocator.Get<IMonoAdapter>().OnAppQuit += OnAppQuit;
            return Task.CompletedTask;
        }

        public async Task InitDataAsync()
        {
            try
            {
                GameDataManager = new GameDataManager();
                await GameDataManager.InitDataAsync();
            }
            catch (Exception ex)
            {
                LogManager.LogError($"{nameof(GameManager)}.{nameof(InitDataAsync)}：{ex.Message}，{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 应用退出事件回调
        /// </summary>
        /// <returns></returns>
        private Task OnAppQuit()
        {
            return GameDataManager.SaveDataAsync();
        }
    }
}
