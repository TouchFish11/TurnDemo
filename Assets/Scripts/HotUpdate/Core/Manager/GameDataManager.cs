using System;
using System.Collections.Generic;
using HotUpdate.Core.Provider;

namespace HotUpdate.Core.Manager
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 游戏数据管理器
    /// </summary>
    public class GameDataManager
    {
        private readonly Dictionary<Type, IDataProvider> _dataProviders = new();

        /// <summary>
        /// 获取提供器
        /// </summary>
        /// <typeparam name="T">该提供器的数据类型</typeparam>
        /// <returns></returns>
        public IDataProvider<T> GetProvider<T>() where T : class
        {
            if (!_dataProviders.ContainsKey(typeof(T)))
            {
                return null;
            }
            return _dataProviders[typeof(T)] as IDataProvider<T>;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetData<T>() where T : class
        {
            if (!_dataProviders.TryGetValue(typeof(T), out var provider))
            {
                return null;
            }

            if (provider is IDataProvider<T> dataTProvider)
            {
                return dataTProvider.GetData();
            }
            
            return null;
        }

        /// <summary>
        /// 注册提供器
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dataProvider"></param>
        public void RegisterProvider(Type type, IDataProvider dataProvider)
        {
            _dataProviders.TryAdd(type, dataProvider);
        }
        
        /// <summary>
        /// 异步加载数据
        /// </summary>
        public async Task LoadDataAsync()
        {
            foreach (var dataProvidersValue in _dataProviders.Values)
            {
                await dataProvidersValue.LoadDataAsync();
            }
        }

        /// <summary>
        /// 异步保存数据
        /// </summary>
        public async Task SaveDataAsync()
        {
            foreach (var provider in _dataProviders.Values)
            {
                await provider.SaveDataAsync();
            }
        }
    }
}
