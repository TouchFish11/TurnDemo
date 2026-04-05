using System;
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
            
            // 创建侧边栏
            var settingOpt = await prefabLoader.GetObjectAsync<SettingOpt>(AbKeyCollection.Ui, ResKeyCollection.SettingOpt, view.Opts);
            
            // 创建设置项
            foreach (var settingItem in settings.Values)
            {
                if (settingItem.IsRange)
                {
                    var sliderEntry = await prefabLoader.GetObjectAsync<SettingSliderEntry>(AbKeyCollection.Ui,
                        ResKeyCollection.SettingSliderEntry, view.Entrys);
                    switch (settingItem.SettingType)
                    {
                        case ESettingType.VolumeValue:
                            sliderEntry.Init("音乐音量", new VolumeSliderViewModel(settings));
                            break;
                        case ESettingType.SFXValue:
                            sliderEntry.Init("音效音量", new SFXSliderViewModel(settings));
                            break;
                    }
                }
                else
                {
                    var DropdownEntry = await prefabLoader.GetObjectAsync<SettingDropdownEntry>(AbKeyCollection.Ui,
                        ResKeyCollection.SettingDrowdownEntry, view.Entrys);
                    switch (settingItem.SettingType)
                    {
                        case ESettingType.VolumeOpen:
                            DropdownEntry.Init("音乐开关", new VolumeOpenDropdownViewModel(settings, settingsConfig));
                            break;
                        case ESettingType.SFXOpen:
                            DropdownEntry.Init("音效开关", new SFXOpenDropdownViewModel(settings, settingsConfig));
                            break;
                        case ESettingType.TypeWriter:
                            DropdownEntry.Init("对话打字机效果", new TypeWriterDropdownViewModel(settings, settingsConfig));
                            break;
                        case ESettingType.TargetFrameRateIndex:
                            DropdownEntry.Init("帧率", new FrameRateDropdownViewModel(settings, settingsConfig));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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
