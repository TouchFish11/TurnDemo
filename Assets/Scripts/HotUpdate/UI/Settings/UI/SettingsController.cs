using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.UI.ViewController;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Settings;
using HotUpdate.Common;

using HotUpdate.UI.Settings.ViewModel;

namespace HotUpdate.UI.Settings.UI
{
    /// <summary>
    /// 设置界面控制器
    /// </summary>
    public class SettingsController : UIController<SettingsView>
    {
        [Inject] private IMainDataManager _mainDataManager;
        [Inject] private ObjectSpawner _objectSpawner;
        private GameSettings _gameSettings;
        private int _mainControllerId;
        
        protected override Task OnInit()
        {
            return ShowSettings();
        }

        protected override Task OnActive()
        {
            return Task.CompletedTask;
        }

        protected override Task OnInactivate()
        {
            // 显示主界面
            return uiManager.SetViewActive(_mainControllerId, true);
        }

        private async Task ShowSettings()
        {
            // 加载游戏设置配置
            var settingsConfig = _mainDataManager.GameSettingsConfig;
            // 获取用户游戏设置数据
            var settings = _mainDataManager.GameSettings;
            // 创建侧边栏
            var settingOpt = await _objectSpawner.SpawnAsync<SettingOpt>(AssetKeys.SettingOpt, view.Opts);
            
            // 创建设置项
            foreach (var settingItem in settings.Values)
            {
                if (settingItem.IsRange)
                {
                    // 获取UI
                    var sliderEntry = await _objectSpawner.SpawnAsync<SettingSliderEntry>(AssetKeys.SettingSliderEntry, view.Entrys);
                    // 获取ViewModel
                    var settingSliderViewModel = SettingsViewModelFactory.CreateSliderViewModel(settingItem.SettingType, settings);
                    // 初始化UI
                    sliderEntry.Obj.Init(SettingsUtil.SettingTypeTOStr(settingItem.SettingType, settingsConfig), settingSliderViewModel);
                }
                else
                {
                    // 获取UI
                    var dropdownEntry = await _objectSpawner.SpawnAsync<SettingDropdownEntry>(
                        AssetKeys.SettingDrowdownEntry, view.Entrys);
                    // 获取ViewModel
                    var settingDropdownViewModel = SettingsViewModelFactory.CreateDropdownViewModel(settingItem.SettingType, settings, settingsConfig);
                    // 初始化UI
                    dropdownEntry.Obj.Init(SettingsUtil.SettingTypeTOStr(settingItem.SettingType, settingsConfig), settingDropdownViewModel);
                }
            }
        }

        protected override void OnButtonClick(string btnName)
        {
            if (btnName == nameof(view.btnClose))
            {
                uiManager.DestroyView(panelId);
            }
        }
    }
}
