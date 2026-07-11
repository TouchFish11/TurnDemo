using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Inputs.ActionAsset;
using Core.Log;
using Core.Music;
using Core.Serialize.Binary;
using Core.Serialize.Json;
using Core.Utility;
using HotUpdate.Base.Attributes;
using HotUpdate.Base.Collection;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Settings;
using HotUpdate.Common.Config.Settings;

using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Main.Data
{
    [DataManagerId(typeof(IMainDataManager))]
    public class MainDataManager : IMainDataManager
    {
        [Inject] private IBinaryDataManager _binaryDataManager;
        [Inject] private IJsonManager _jsonManager;
        
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
            await _binaryDataManager.LoadConfigAsync(AssetBundleKeys.Gameconfig);
            Logger.LogDebug(ELogTags.Main, $"配置数据加载成功");
            
            // 读取本地音乐数据
            var MusicData = await _binaryDataManager.LoadAsync<MusicData>(FileUtility.LocalMusicDataFileName);
            Logger.LogDebug(ELogTags.Main, $"本地音乐数据加载成功，{MusicData}");
            
            // 读取本地输入数据
            var InputActionContainer = await _binaryDataManager.LoadAsync<MainActionMapDataContainer>(FileUtility.LocalInputDataFileName);
            Logger.LogDebug(ELogTags.Main, $"本地输入数据加载成功，{InputActionContainer}");

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
            using var handle = await GameAsset.LoadAssetAsync<TextAsset>(AssetKeys.GameSettingsConfig);
            GameSettingsConfig = _jsonManager.FromJson<GameSettingsConfig>(handle.Asset.text);
        }

        public void SaveData()
        {
            if(MainDataCollection != null)
            {
                // 保存音乐数据
                _binaryDataManager.Save(FileUtility.LocalMusicDataFileName, MainDataCollection.MusicData);
                Logger.LogDebug(ELogTags.Main, $"音乐数据保存成功，{FileUtility.LocalMusicDataFileName}");
            }
            
            // 保存输入数据
            if (MainDataCollection != null)
            {
                _binaryDataManager.Save(FileUtility.LocalInputDataFileName, MainDataCollection.InputActionContainer);
                Logger.LogDebug(ELogTags.Main, $"输入数据保存成功，{FileUtility.LocalInputDataFileName}");
            }

            if (GameSettings != null)
            {
                // 保存设置数据
                _jsonManager.SaveToJson(GameSettings, $"{PathUtility.GetUserDataLocalSavePath(FileUtility.GameSettingFileName)}", settings:NewtonsoftJsonUtility.SerializerSettings);
                Logger.LogDebug(ELogTags.Main, $"游戏设置数据保存成功，{GameSettings}");
            }
        }

        public async Task SaveDataAsync()
        {
            if (MainDataCollection != null)
            {
                // 保存音乐数据
                await _binaryDataManager.SaveAsync(FileUtility.LocalMusicDataFileName, MainDataCollection.MusicData);
                Logger.LogDebug(ELogTags.Main, $"音乐数据保存成功，{FileUtility.LocalMusicDataFileName}");
            }
            
            if (MainDataCollection != null)
            {
                // 保存输入数据
                await _binaryDataManager.SaveAsync(FileUtility.LocalInputDataFileName, MainDataCollection.InputActionContainer);
                Logger.LogDebug(ELogTags.Main, $"输入数据保存成功，{FileUtility.LocalInputDataFileName}");
            }

            if (GameSettings != null)
            {
                // 保存设置数据
                await _jsonManager.SaveToJsonAsync(GameSettings, $"{PathUtility.GetUserDataLocalSavePath(FileUtility.GameSettingFileName)}", settings:NewtonsoftJsonUtility.SerializerSettings);
                Logger.LogDebug(ELogTags.Main, $"游戏设置数据保存成功，{GameSettings}");
            }
        }
    }
}
