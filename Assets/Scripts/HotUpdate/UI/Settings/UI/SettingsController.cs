using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.UI;
using Core.UI.ViewController;
using HotUpdate.Base.Data;
using HotUpdate.Base.Settings;
using HotUpdate.Base.UI;
using HotUpdate.UI.Settings.Handlers;
using HotUpdate.UI.Settings.ViewModel;

namespace HotUpdate.UI.Settings.UI
{
    /// <summary>
    /// 设置界面控制器。
    /// 侧边栏生成 4 个分类页签，右侧按当前分类渲染对应设置条目；
    /// 切页签时释放旧条目并重建；按钮型设置点击触发一次性 handler。
    /// </summary>
    public class SettingsController : UIController<SettingsView>, IBlockOperation
    {
        [Inject] private IMainDataProvider mainDataProvider;
        [Inject] private ObjectSpawner _objectSpawner;
        [Inject] private IUIService _uiService;
        [Inject] private SettingHandlerRegistry _settingHandlerRegistry;

        // 分类 → 页签组件
        private readonly Dictionary<ESettingCategory, SettingOpt> _tabs = new();
        // 当前分类下已生成的条目（切页签/刷新时释放）
        private readonly List<UIBehaviourBase> _spawnedEntries = new();
        // 当前选中的分类
        private ESettingCategory _currentCategory;

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
            return Task.CompletedTask;
        }

        private async Task ShowSettings()
        {
            var settingsConfig = mainDataProvider.GameSettingsConfig;
            var settings = mainDataProvider.GameSettings;

            // 生成 4 个分类页签
            foreach (ESettingCategory category in Enum.GetValues(typeof(ESettingCategory)))
            {
                var tab = await _objectSpawner.SpawnAsync<SettingOpt>(AssetKeys.SettingOpt, view.Opts);
                tab.Init(category, OnTabSelected);
                _tabs[category] = tab;
            }

            // 默认显示第一个分类（画面）
            _currentCategory = ESettingCategory.Graphics;
            RefreshTabSelection();
            await RenderEntries(settingsConfig, settings);
        }

        private async void OnTabSelected(ESettingCategory category)
        {
            if (_currentCategory == category)
                return;

            _currentCategory = category;
            RefreshTabSelection();
            await RenderEntries(mainDataProvider.GameSettingsConfig, mainDataProvider.GameSettings);
        }

        /// <summary>
        /// 刷新页签高亮（只把当前分类设为选中，其余取消）
        /// </summary>
        private void RefreshTabSelection()
        {
            foreach (var (category, opt) in _tabs)
            {
                opt.SetSelected(category == _currentCategory);
            }
        }

        private async Task RenderEntries(GameSettingsConfig settingsConfig, GameSettings settings)
        {
            // 先释放旧条目，再按当前分类重新生成
            _objectSpawner.Release(_spawnedEntries);
            _spawnedEntries.Clear();

            foreach (var definition in settingsConfig.Settings)
            {
                if (definition.Category != _currentCategory)
                    continue;

                switch (definition.Widget)
                {
                    case ESettingWidget.Slider:
                        var sliderEntry = (SettingSliderEntry)await _objectSpawner.SpawnAsync<UIBehaviourBase>(AssetKeys.SettingSliderEntry, view.Entrys);
                        var sliderViewModel = SettingsViewModelFactory.CreateSliderViewModel(settings, definition);
                        sliderEntry.Init(definition, sliderViewModel);
                        OnSettingDataChanged(sliderViewModel, definition.Type);
                        _spawnedEntries.Add(sliderEntry);
                        break;

                    case ESettingWidget.Dropdown:
                        var dropdownEntry = (SettingDropdownEntry)await _objectSpawner.SpawnAsync<UIBehaviourBase>(AssetKeys.SettingDrowdownEntry, view.Entrys);
                        var dropdownViewModel = SettingsViewModelFactory.CreateDropdownViewModel(settings, definition);
                        dropdownEntry.Init(definition, dropdownViewModel);
                        OnSettingDataChanged(dropdownViewModel, definition.Type);
                        _spawnedEntries.Add(dropdownEntry);
                        break;

                    case ESettingWidget.Button:
                        var buttonEntry = (SettingButtonEntry)await _objectSpawner.SpawnAsync<UIBehaviourBase>(AssetKeys.SettingButtonEntry, view.Entrys);
                        buttonEntry.Init(definition, () => OnButtonPressed(definition.Type));
                        _spawnedEntries.Add(buttonEntry);
                        break;
                }
            }
        }

        /// <summary>
        /// 按钮型设置被点击：从注册表取对应 handler 执行一次性动作
        /// </summary>
        private async void OnButtonPressed(ESettingType type)
        {
            ((IButtonSettingHandler)_settingHandlerRegistry.Get(type)).Execute();

            // 重置后刷新 UI，让滑块/下拉回到默认显示
            if (type == ESettingType.Reset)
            {
                await RenderEntries(mainDataProvider.GameSettingsConfig, mainDataProvider.GameSettings);
            }
        }

        /// <summary>
        /// 订阅 ViewModel 的值变化：写回 GameSettings 持久化，并调用对应 handler 让设置立即生效
        /// </summary>
        private void OnSettingDataChanged(IDisposable viewModel, ESettingType settingType)
        {
            if (viewModel == null)
                return;

            switch (viewModel)
            {
                case SettingSliderViewModel settingSliderViewModel:
                    settingSliderViewModel.Progress.Subscribe(value =>
                    {
                        mainDataProvider.GameSettings.SetFloat(settingType, value);
                        ((ISliderSettingHandler)_settingHandlerRegistry.Get(settingType)).Excute(value);
                    });
                    break;
                case SettingDropdownViewModel settingDropdownViewModel:
                    settingDropdownViewModel.OptionIndex.Subscribe(optionIndex =>
                    {
                        mainDataProvider.GameSettings.SetInt(settingType, optionIndex);
                        ((IDropdownSettingHandler)_settingHandlerRegistry.Get(settingType)).Execute(optionIndex);
                    });
                    break;
            }
        }

        protected override void OnButtonClick(string btnName)
        {
            if (btnName == nameof(view.btnClose))
            {
                _uiService.CloseAsync(panelId, true, true);
            }
        }
    }
}
