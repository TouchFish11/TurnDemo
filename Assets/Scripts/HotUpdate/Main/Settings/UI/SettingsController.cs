using System.Reflection;
using System.Threading.Tasks;
using Core.Service;
using Core.UI.MVC;
using HotUpdate.Common;
using HotUpdate.Core.Main;
using HotUpdate.Core.Main.Settings;
using HotUpdate.Core.Main.Settings.ViewModel;
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
            // 创建设置UI
            var fieldInfos = settings.GetType().GetFields();
            foreach (var fieldInfo in fieldInfos)
            {
                if(!fieldInfo.IsDefined(typeof(SettingTypeAttribute), false)) continue;
                var settingTypeAttribute = fieldInfo.GetCustomAttribute<SettingTypeAttribute>();
                if (settingTypeAttribute.IsRange)
                {
                    var sliderEntry = await prefabLoader.GetObjectAsync<SettingSliderEntry>(AbKeyCollection.Ui,
                        ResKeyCollection.SettingSliderEntry, view.Entrys);
                    var settingSliderViewModel = new SettingSliderViewModel(settings);
                    sliderEntry.Init("测试滑动条", settingSliderViewModel);
                }
                else
                {
                    var drowdownEntry = await prefabLoader.GetObjectAsync<SettingDrowdownEntry>(AbKeyCollection.Ui,
                        ResKeyCollection.SettingDrowdownEntry, view.Entrys);
                    var settingDrowdownViewModel = new SettingDrowdownViewModel(settings, settingsConfig);
                    drowdownEntry.Init("测试下拉列表", settingDrowdownViewModel);
                }
            }
        }
    }
}
