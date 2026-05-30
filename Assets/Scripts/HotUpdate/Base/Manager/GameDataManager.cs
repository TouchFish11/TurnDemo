using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Core.DI;
using Core.HotUpdate;
using Core.Mono;
using HotUpdate.Base.Attributes;

namespace HotUpdate.Base.Manager
{
    /// <summary>
    /// 游戏数据管理器
    /// </summary>
    public class GameDataManager : IGameDataManager, IApplicationExitNotify
    {
        private readonly List<IDataManager> _dataManagers = new();

        public int QuitPriority => 0;
        
        public GameDataManager(IMonoAdapter monoAdapter, IHotUpdateManager hotUpdateManager)
        {
            monoAdapter.AddApplicationExitNotify(this);
            Init(hotUpdateManager);
        }

        /// <summary>
        /// 初始化管理器
        /// </summary>
        /// <param name="hotUpdateManager"></param>
        private void Init(IHotUpdateManager hotUpdateManager)
        {
            foreach (var hotAssembly in hotUpdateManager.GetHotAssemblies())
            {
                foreach (var type in hotAssembly.GetTypes())
                {
                    if (!typeof(IDataManager).IsAssignableFrom(type) && type.IsClass)
                        continue;

                    var dataManagerIdAttribute = type.GetCustomAttribute<DataManagerIdAttribute>();
                    if (dataManagerIdAttribute == null)
                        continue;
                    
                    var dataManager = DIContainer.Create(dataManagerIdAttribute.DataManagerIdMapType, type, true) as IDataManager;
                    _dataManagers.Add(dataManager);
                }
            }
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
