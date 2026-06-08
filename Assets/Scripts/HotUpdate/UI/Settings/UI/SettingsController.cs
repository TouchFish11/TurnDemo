using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.UI.ViewController;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Settings;
using HotUpdate.Base.UI;
using HotUpdate.UI.Settings.Handlers;
using HotUpdate.UI.Settings.ViewModel;

namespace HotUpdate.UI.Settings.UI
{
    /// <summary>
    /// 设置界面控制器
    /// </summary>
    public class SettingsController : UIController<SettingsView>, IBlockOperation
    {
        [Inject] private IMainDataManager _mainDataManager;
        [Inject] private ObjectSpawner _objectSpawner;
        [Inject] private IUIService _uiService;
        private GameSettings _gameSettings;

        private readonly Dictionary<ESettingType, ISettingHandler> _settingHandlers = new()
        {
            { ESettingType.VolumeValue , DIContainer.Create<SliderSettingHandler>()},
            { ESettingType.SFXValue , DIContainer.Create<SliderSettingHandler>()},
            { ESettingType.TargetFrameRateIndex , DIContainer.Create<FrameRateSettingHandler>()},
            { ESettingType.TypeWriter , DIContainer.Create<DropdownSettingHandler>()},
            { ESettingType.VolumeOpen , DIContainer.Create<DropdownSettingHandler>()},
            { ESettingType.SFXOpen , DIContainer.Create<DropdownSettingHandler>()},
        };
        
        public bool BlockOperation { get; } = true;

        protected override bool IsCursorVisible { get; set; } = true;

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
            return _uiService.ShowAsync(_uiService.GetPanel(EUIPanelId.MainPanel).PanelId);
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
            // TODO：可以优化
            foreach (var settingItem in settings.Values)
            {
                if (settingItem.IsRange)
                {
                    // 获取UI
                    var sliderEntry = await _objectSpawner.SpawnAsync<SettingSliderEntry>(AssetKeys.SettingSliderEntry, view.Entrys);
                    // 获取ViewModel
                    var settingSliderViewModel = SettingsViewModelFactory.CreateSliderViewModel(settingItem.SettingType, settings);
                    // 初始化UI
                    sliderEntry.Init(SettingsUtil.SettingTypeToStr(settingItem.SettingType, settingsConfig), settingSliderViewModel);
                    OnSettingDataChanged(settingSliderViewModel, settingItem.SettingType);
                }
                else
                {
                    // 获取UI
                    var dropdownEntry = await _objectSpawner.SpawnAsync<SettingDropdownEntry>(
                        AssetKeys.SettingDrowdownEntry, view.Entrys);
                    // 获取ViewModel
                    var settingDropdownViewModel = SettingsViewModelFactory.CreateDropdownViewModel(settingItem.SettingType, settings, settingsConfig);
                    // 初始化UI
                    dropdownEntry.Init(SettingsUtil.SettingTypeToStr(settingItem.SettingType, settingsConfig), settingDropdownViewModel);
                    OnSettingDataChanged(settingDropdownViewModel, settingItem.SettingType);
                }
            }
        }

        private void OnSettingDataChanged(IDisposable viewModel, ESettingType settingType)
        {
            if(viewModel == null)
                return;
            
            switch (viewModel)
            {
                case SettingSliderViewModel settingSliderViewModel:
                    settingSliderViewModel.Progress.Subscribe(value =>
                    {
                        var progress = value / SettingsUtil.SLIDER_MULTIPLIER;
                        _mainDataManager.GameSettings[settingType] = progress;
                        ((ISliderSettingHandler)_settingHandlers[settingType]).Excute(progress);
                    });
                    break;
                case SettingDropdownViewModel settingDropdownViewModel:
                    settingDropdownViewModel.OptionIndex.Subscribe(optionIndex =>
                    {
                        _mainDataManager.GameSettings[settingType] = optionIndex; 
                        ((IDropdownSettingHandler)_settingHandlers[settingType]).Execute(optionIndex);
                    });
                    break;
            }
        }

        protected override void OnButtonClick(string btnName)
        {
            if (btnName == nameof(view.btnClose))
            {
                _uiService.CloseAsync(panelId, true);
            }
        }
    }
}
