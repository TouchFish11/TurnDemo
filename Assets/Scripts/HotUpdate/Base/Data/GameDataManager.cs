using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Core.DI;
using Core.HotUpdate;
using Core.Log;
using Core.Mono;
using HotUpdate.Base.Attributes;

namespace HotUpdate.Base.Data
{
    /// <summary>
    /// 游戏数据管理器
    /// </summary>
    public class GameDataManager : IGameDataManager, IApplicationExitNotify
    {
        private readonly List<IPersistable> _dataProviders = new();

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
                    if (!typeof(IDataProvider).IsAssignableFrom(type) && type.IsClass)
                        continue;

                    var dataManagerIdAttribute = type.GetCustomAttribute<DataProviderIdAttribute>();
                    if (dataManagerIdAttribute == null)
                        continue;
                    
                    DIContainer.BindSingleton(dataManagerIdAttribute.DataManagerIdMapType, type);
                    var dataManager = DIContainer.Resolve(type) as IPersistable;
                    _dataProviders.Add(dataManager);
                }
            }
        }

        public async Task LoadConfigAsync()
        {
            foreach (var dataProvider in _dataProviders)
            {
                if (dataProvider is ISOConfigSources sources)
                {
                    try
                    {
                        await sources.LoadConfigAsync();
                    }
                    catch (Exception e)
                    {
                        Logger.LogException(ELogTags.Setting, e);
                    }
                }
            }
        }
        
        /// <summary>
        /// 异步加载数据
        /// </summary>
        public async Task LoadDataAsync()
        {
            foreach (var dataManager in _dataProviders)
            {
                try
                {
                    await dataManager.LoadDataAsync();
                }
                catch (Exception e)
                {
                    Logger.LogException(ELogTags.Setting, e);
                }
            }
        }

        /// <summary>
        /// 异步保存数据
        /// </summary>
        public async Task SaveDataAsync()
        {
            foreach (var dataManager in _dataProviders)
            {
                try
                {
                    await dataManager.SaveDataAsync();
                }
                catch (Exception e)
                {
                    Logger.LogException(ELogTags.Setting, e);
                }
            }
        }

        public void LoadData()
        {
            foreach (var dataProvider in _dataProviders)
            {
                dataProvider.LoadData();
            }
        }

        /// <summary>
        /// 同步保存数据
        /// </summary>
        public void SaveData()
        {
            foreach (var provider in _dataProviders)
            {
                provider.SaveData();
            }
        }
        
        public void OnAppQuit()
        {
            SaveData();
        }
    }
}
