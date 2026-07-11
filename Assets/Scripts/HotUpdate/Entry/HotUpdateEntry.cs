using System;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Log;
using Core.Serialize.Json;
using Core.UI;
using Core.Utility;
using HotUpdate.Base.Module;
using HotUpdate.Base.Settings;
using HotUpdate.Base.UI;
using HotUpdate.Common.Config.Settings;
using HotUpdate.Game.Main;
using HotUpdate.UI.Begin;
using UnityEngine;
using UnityEngine.Scripting;
using Logger = Core.Log.Logger;

namespace HotUpdate.Entry
{
    /// <summary>
    /// 热更新入口
    /// </summary>
    [Preserve]
    public class HotUpdateEntry : MonoBehaviour
    {
        private IUIService _uiService;
        private IUIManager _uiManager;
        private PlayerInitializer _playerInitializer;
        private IJsonManager _jsonManager;
        private ModuleService _moduleService;
        
        private void Awake()
        {
            // 注册模块相关内容
            _moduleService = DIContainer.Resolve<ModuleService>();
            _moduleService.RegisterModules();
            
            _uiService = DIContainer.Resolve<IUIService>();
            _uiManager = DIContainer.Resolve<IUIManager>();
            _playerInitializer = DIContainer.Resolve<PlayerInitializer>();
            _jsonManager = DIContainer.Resolve<IJsonManager>();    
        }

        private async void OnEnable()
        {
            try
            {
                // 在OnEnable执行run逻辑，而不是在Start，因为Start执行晚于该对象的释放
                await Run();
            }
            catch (Exception e)
            {
                Logger.LogError(ELogTags.HotUpdateEntry, $"{nameof(HotUpdateEntry)}: hotfix entry error,{e.Message}");
            }
        }

        /// <summary>
        /// 运行
        /// </summary>
        private async Task Run()
        {
            try
            {
                await _moduleService.InitModulesAsync();
                // 初始化游戏设置
                await InitSettings();
                // 初始化UI管理器，创建画布和UI相机
                await _uiManager.InitUIManagerAsync(AssetKeys.UIRoot);
                // 显示开始界面
                var controller = await _uiService.OpenAsync(EUIPanelId.BeginPanel, E_UILayer.Mid) as BeginController;
                // 进入游戏
                controller.OnClickEnterGame += EnterGame;
                // 检查更新
                controller.CheckUpdate();
            }
            catch (Exception e)
            {
                Logger.LogError(ELogTags.HotUpdateEntry, $"{nameof(HotUpdateEntry)}: Error occurred while running the hot update entry,{e.Message}");
            }
        }

        /// <summary>
        /// 初始化设置
        /// </summary>
        private async Task InitSettings()
        {
            using var handle = await GameAsset.LoadAssetAsync<TextAsset>(AssetKeys.GameSettingsConfig);
            var gameSettingsConfig = _jsonManager.FromJson<GameSettingsConfig>(handle.Asset.text, settings:NewtonsoftJsonUtility.SerializerSettings);
            var settings = await _jsonManager.FromJsonAsync<GameSettings>(PathUtility.GetUserDataLocalSavePath(FileUtility.GameSettingFileName), settings:NewtonsoftJsonUtility.SerializerSettings);
            
            // TODO：逻辑可优化
            SettingsService.SetFrameRate(gameSettingsConfig.framerates[(int)settings[ESettingType.TargetFrameRateIndex]]);
            Application.runInBackground = true;
            
            // ...
        }
        
        /// <summary>
        /// 进入游戏
        /// </summary>
        private async Task EnterGame()
        {
            try
            {
                // 初始化玩家相关内容
                await _playerInitializer.InitPlayerAsync();
                // 初始化全局消息界面
                await _uiService.OpenAsync(EUIPanelId.GlobalPanel, E_UILayer.Mid);
                // 打开主界面
                await _uiService.OpenAsync(EUIPanelId.MainPanel, E_UILayer.Mid);
            }
            catch (Exception e)
            {
                Logger.LogError(ELogTags.HotUpdateEntry, $"{nameof(HotUpdateEntry)}:Entry game error, {e.Message}");
            }
        }
    }
}
