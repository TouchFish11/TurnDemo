using System;
using System.Collections.Generic;
using Core.Input.ActionAsset;
using Core.Input.CoreListen;
using Core.Log;
using Core.Music;
using Core.Serialize.Binary;
using Core.Service;
using Core.Utility;
using HotUpdate.Common;
using HotUpdate.Core.Provider;

namespace HotUpdate.Core.Manager
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 游戏数据管理器
    /// </summary>
    public class GameDataManager
    {
        private readonly IBinaryDataManager _binaryDataManager = ServiceLocator.Get<IBinaryDataManager>();
        private readonly Dictionary<Type, IDataProvider> _dataProviders = new();

        public IDataProvider<T> GetDataProvider<T>() where T : class
        {
            if (!_dataProviders.ContainsKey(typeof(T)))
            {
                return null;
            }
            return _dataProviders[typeof(T)] as IDataProvider<T>;
        }

        public async System.Threading.Tasks.Task<T> GetData<T>() where T : class
        {
            if (!_dataProviders.TryGetValue(typeof(T), out var provider))
            {
                return null;
            }

            if (provider is IDataProvider<T> dataTProvider)
            {
                return await dataTProvider.GetDataAsync();
            }
            
            return null;
        }

        public void AddDataProvider(Type type, IDataProvider dataProvider)
        {
            _dataProviders.TryAdd(type, dataProvider);
        }
        
        public async Task InitDataAsync()
        {
            _binaryDataManager.AddConfig(EConfigLoadType.Excel, async loader =>
            {
                await loader.LoadConfigAsync<RoleInfoContainer, RoleInfo>();
                await loader.LoadConfigAsync<MonsterInfoContainer, MonsterInfo>();
                await loader.LoadConfigAsync<SkillInfoContainer, SkillInfo>();
                await loader.LoadConfigAsync<StatusInfoContainer, StatusInfo>();
                await loader.LoadConfigAsync<DialogueInfoContainer, DialogueInfo>();
                await loader.LoadConfigAsync<BranchInfoContainer, BranchInfo>();
                await loader.LoadConfigAsync<TaskInfoContainer, TaskInfo>();
                await loader.LoadConfigAsync<TaskConditionInfoContainer, TaskConditionInfo>();
                await loader.LoadConfigAsync<NpcInfoContainer, NpcInfo>();
            
                await loader.LoadConfigAsync<ActivityInfoContainer,ActivityInfo>();
                await loader.LoadConfigAsync<ItemInfoContainer,ItemInfo>();
            });
            
            // 加载二进制配置
            await _binaryDataManager.LoadConfigAsync(AbKeyCollection.Gameconfig);
            LogManager.Log($"配置数据加载成功");
            
            // 读取本地音乐数据
            MusicData = await _binaryDataManager.LoadAsync<MusicData>(FileUtility.LocalMusicDataFileName);
            LogManager.Log($"本地音乐数据加载成功，{MusicData}");
            
            // 读取本地输入数据
            InputActionContainer = await _binaryDataManager.LoadAsync<MainActionMapDataContainer>(FileUtility.LocalInputDataFileName);
            LogManager.Log($"本地输入数据加载成功，{InputActionContainer}");
        }

        public async Task SaveDataAsync()
        {
            // 保存音乐数据
            await _binaryDataManager.SaveAsync(FileUtility.LocalMusicDataFileName, MusicData);
            LogManager.Log($"音乐数据保存成功，{MusicData}");
            
            // 保存输入数据
            await _binaryDataManager.SaveAsync(FileUtility.LocalInputDataFileName, InputActionContainer);
            LogManager.Log($"输入数据保存成功，{InputActionContainer}");
            
            foreach (var provider in _dataProviders.Values)
            {
                await provider.SaveDataAsync();
            }
        }
        
        /// <summary>
        /// 音乐数据
        /// </summary>
        public MusicData MusicData { get; private set; }

        /// <summary>
        /// 主动作行为映射数据容器
        /// </summary>
        public MainActionMapDataContainer InputActionContainer { get; private set; }
        
        /// <summary>
        /// 输入数据集合
        /// </summary>
        public InputDataContainer InputDataContainer { get; private set; }
    }
}
