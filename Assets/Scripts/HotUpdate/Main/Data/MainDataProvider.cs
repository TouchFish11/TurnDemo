using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Input.ActionAsset;
using Core.Log;
using Core.Music;
using Core.Serialize.Binary;
using Core.Serialize.Json;
using Core.Service;
using Core.Utility;
using HotUpdate.Common;
using HotUpdate.Core.Main;
using HotUpdate.Core.Main.Settings;
using UnityEngine;

namespace HotUpdate.Main.Data
{
    /// <summary>
    /// 主模块数据提供器
    /// </summary>
    public class MainDataProvider : IMainDataProvider
    {
        private readonly IBinaryDataManager _binaryDataManager;
        private readonly IJsonManager _jsonManager;
        
        /// <summary>
        /// 主数据集合
        /// </summary>
        public IMainDataCollection MainDataCollection { get; private set; }
        
        /// <summary>
        /// 游戏设置数据
        /// </summary>
        public GameSettings GameSettings { get; private set; }

        /// <summary>
        /// 游戏设置配置
        /// </summary>
        public GameSettingsConfig GameSettingsConfig { get; private set; }

        public MainDataProvider(IBinaryDataManager binaryDataManager, IJsonManager jsonManager)
        {
            _binaryDataManager = binaryDataManager;
            _jsonManager = jsonManager;
        }

        public async Task LoadDataAsync()
        {
            // 添加配置数据
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
            var MusicData = await _binaryDataManager.LoadAsync<MusicData>(FileUtility.LocalMusicDataFileName);
            LogManager.Log($"本地音乐数据加载成功，{MusicData}");
            
            // 读取本地输入数据
            var InputActionContainer = await _binaryDataManager.LoadAsync<MainActionMapDataContainer>(FileUtility.LocalInputDataFileName);
            LogManager.Log($"本地输入数据加载成功，{InputActionContainer}");

            // 构造主数据集合
            MainDataCollection = new MainDataCollection
            {
                InputActionContainer = InputActionContainer,
                InputDataContainer = null,
                MusicData = MusicData,
            };
            
            // 读取游戏设置数据
            GameSettings = await _jsonManager.FromJsonAsync<GameSettings>($"{PathUtility.GetUserDataLocalSavePath(FileUtility.GameSettingFileName)}", settings:NewtonsoftJsonUtility.SerializerSettings);
            
            // 读取游戏设置数据配置
            var ab = await ServiceLocator.Get<IAssetBundleManager>().LoadBundleAsync(AbKeyCollection.Gameconfig);
            var textAsset = ab.LoadAsset<TextAsset>(ResKeyCollection.GameSettingsConfig);
            ServiceLocator.Get<IAssetBundleManager>().UnloadBundle(AbKeyCollection.Gameconfig);
            GameSettingsConfig = _jsonManager.FromJson<GameSettingsConfig>(textAsset.text);
        }

        public async Task SaveDataAsync()
        {
            // 保存音乐数据
            await _binaryDataManager.SaveAsync(FileUtility.LocalMusicDataFileName, MainDataCollection.MusicData);
            LogManager.Log($"{nameof(MainDataProvider)}.{nameof(SaveDataAsync)}:音乐数据保存成功，{FileUtility.LocalMusicDataFileName}");
            
            // 保存输入数据
            await _binaryDataManager.SaveAsync(FileUtility.LocalInputDataFileName, MainDataCollection.InputActionContainer);
            LogManager.Log($"{nameof(MainDataProvider)}.{nameof(SaveDataAsync)}:输入数据保存成功，{FileUtility.LocalInputDataFileName}");
            
            // 保存设置数据
            await _jsonManager.SaveToJsonAsync(GameSettings, $"{PathUtility.GetUserDataLocalSavePath(FileUtility.GameSettingFileName)}", settings:NewtonsoftJsonUtility.SerializerSettings);
            LogManager.Log($"{nameof(MainDataProvider)}.{nameof(SaveDataAsync)}:游戏设置数据保存成功，{GameSettings}");
        }

        public void SaveData()
        {
            // 保存音乐数据
             _binaryDataManager.Save(FileUtility.LocalMusicDataFileName, MainDataCollection.MusicData);
             LogManager.Log($"{nameof(MainDataProvider)}.{nameof(SaveData)}:音乐数据保存成功，{FileUtility.LocalMusicDataFileName}");
            
            // 保存输入数据
             _binaryDataManager.Save(FileUtility.LocalInputDataFileName, MainDataCollection.InputActionContainer);
             LogManager.Log($"{nameof(MainDataProvider)}.{nameof(SaveData)}:输入数据保存成功，{FileUtility.LocalInputDataFileName}");
             
             // 保存设置数据
             _jsonManager.SaveToJson(GameSettings, $"{PathUtility.GetUserDataLocalSavePath(FileUtility.GameSettingFileName)}", settings:NewtonsoftJsonUtility.SerializerSettings);
             LogManager.Log($"{nameof(MainDataProvider)}.{nameof(SaveDataAsync)}:游戏设置数据保存成功，{GameSettings}");
        }

        public IMainDataCollection GetData()
        {
            return MainDataCollection;
        }
    }
}
