using System;
using Core.Log;
using Core.Mono;

namespace HotUpdate.Core.Manager
{
    using Task = System.Threading.Tasks.Task;
    
    /// <summary>
    /// 游戏管理器
    /// </summary>
    public class GameManager : IGameManager, IApplicationExitNotify
    {
        public int QuitPriority => 0;
        // 游戏数据管理器
        public GameDataManager GameDataManager { get; }

        public GameManager(IMonoAdapter monoAdapter)
        {
            GameDataManager = new GameDataManager();
            monoAdapter.AddApplicationExitNotify(this);
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
        
        public async void OnAppQuit()
        {
            try
            {
                await GameDataManager.SaveDataAsync();
                LogManager.LogError($"{nameof(GameManager)}.{nameof(OnAppQuit)}:数据保存成功");
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(GameManager)}.{nameof(OnAppQuit)}:数据保存错误，{e.Message}");
            }
        }
    }
}
