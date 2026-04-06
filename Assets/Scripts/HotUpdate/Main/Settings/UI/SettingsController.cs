using System.Threading.Tasks;
using Core.Service;
using Core.UI.MVC;
using HotUpdate.Common;
using HotUpdate.Core.Main;
using HotUpdate.Core.Main.Settings;
using HotUpdate.Core.Manager;
using HotUpdate.Core.UI.MVC;

namespace HotUpdate.Main.Settings.UI
{
    /// <summary>
    /// 设置界面控制器
    /// </summary>
    public class SettingsController : UIController<SettingsView, SettingsModel>
    {
        protected override Task OnInit()
        {
            return ShowSettings();
        }
        
        protected override Task OnShow()
        {
            return Task.CompletedTask;
        }

        protected override Task OnHide()
        {
            // 显示主界面
            return uiManager.SetViewActive(uiManager.GetController<IMainController>(), true);
        }

        private async Task ShowSettings()
        {
            // 加载游戏设置配置
            var settingsConfig = ServiceLocator.Get<IGameManager>().GameDataManager.GetProvider<IMainDataProvider>().GameSettingsConfig;
            // 获取用户游戏设置数据
            var settings = ServiceLocator.Get<IGameManager>().GameDataManager.GetProvider<IMainDataProvider>().GameSettings;
            // 创建侧边栏
            var settingOpt = await prefabLoader.GetObjectAsync<SettingOpt>(AbKeyCollection.Ui, ResKeyCollection.SettingOpt, view.Opts);
            
            // 创建设置项
            foreach (var settingItem in settings.Values)
            {
                if (settingItem.IsRange)
                {
                    // 获取UI
                    var sliderEntry = await prefabLoader.GetObjectAsync<SettingSliderEntry>(AbKeyCollection.Ui, ResKeyCollection.SettingSliderEntry, view.Entrys);
                    // 获取ViewModel
                    var settingSliderViewModel = SettingsViewModelFactory.CreateSliderViewModel(settingItem.SettingType, settings);
                    // 初始化UI
                    sliderEntry.Init(SettingsUtil.SettingTypeTOStr(settingItem.SettingType, settingsConfig), settingSliderViewModel);
                }
                else
                {
                    // 获取UI
                    var dropdownEntry = await prefabLoader.GetObjectAsync<SettingDropdownEntry>(AbKeyCollection.Ui,
                        ResKeyCollection.SettingDrowdownEntry, view.Entrys);
                    // 获取ViewModel
                    var settingDropdownViewModel = SettingsViewModelFactory.CreateDropdownViewModel(settingItem.SettingType, settings, settingsConfig);
                    // 初始化UI
                    dropdownEntry.Init(SettingsUtil.SettingTypeTOStr(settingItem.SettingType, settingsConfig), settingDropdownViewModel);
                }
            }
        }

        protected override void ButtonOnClick(string btnName)
        {
            if (btnName == nameof(view.btnClose))
            {
                uiManager.DestroyView(AbKeyCollection.Ui, this);
            }
        }
    }
}
