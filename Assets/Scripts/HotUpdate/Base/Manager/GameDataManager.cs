using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Mono;

namespace HotUpdate.Base.Manager
{
    /// <summary>
    /// 游戏数据管理器
    /// </summary>
    public class GameDataManager : IGameDataManager, IApplicationExitNotify
    {
        private readonly List<IDataManager> _dataManagers = new();

        public int QuitPriority => 0;
        
        public GameDataManager(IMonoAdapter monoAdapter)
        {
            monoAdapter.AddApplicationExitNotify(this);
        }
        
        /// <summary>
        /// 异步加载数据
        /// </summary>
        public async Task LoadDataAsync()
        {
            foreach (var dataManager in _dataManagers)
            {
                await dataManager.LoadDataAsync();
            }
        }

        /// <summary>
        /// 异步保存数据
        /// </summary>
        public async Task SaveDataAsync()
        {
            foreach (var dataManager in _dataManagers)
            {
                await dataManager.SaveDataAsync();
            }
        }

        /// <summary>
        /// 同步保存数据
        /// </summary>
        public void SaveData()
        {
            foreach (var dataManager in _dataManagers)
            {
                dataManager.SaveData();
            }
        }
        
        public void OnAppQuit()
        {
            SaveData();
        }
    }
}
