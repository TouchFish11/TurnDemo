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
    public class GameManager : IGameManager
    {
        // 游戏数据管理器
        public GameDataManager GameDataManager { get; }

        public GameManager(IMonoAdapter monoAdapter)
        {
            GameDataManager = new GameDataManager();
            monoAdapter.OnAppQuit += OnAppQuit;
        }
        
        public async Task InitDataAsync()
        {
            try
            {
                await GameDataManager.LoadDataAsync();
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
