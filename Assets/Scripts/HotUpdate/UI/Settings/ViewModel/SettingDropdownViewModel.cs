using System;
using System.Collections.Generic;
using Core.UI;
using HotUpdate.Base.Settings;

namespace HotUpdate.UI.Settings.ViewModel
{
    /// <summary>
    /// 下拉列表设置ViewModel
    /// </summary>
    public abstract class SettingDropdownViewModel : IDisposable
    {
        protected GameSettings _settings;
        protected GameSettingsConfig _settingsConfig;
        
        public ReactiveProperty<int> OptionIndex { get; private set; } = new();
        public List<string> Options { get; protected set; } = new();
        
        protected SettingDropdownViewModel(GameSettings settings, GameSettingsConfig settingsConfig)
        {
            _settings = settings;
            _settingsConfig = settingsConfig;
        }
        
        protected abstract void OnSettingsChanged(GameSettings settings);

        /// <summary>
        /// 刷新UI显示，UI初始化时需主动调用该方法，拉取数据来显示
        /// </summary>
        public abstract void RefleshUI();

        public void Dispose()
        {
            _settings.OnDataChanged -= OnSettingsChanged;
            OptionIndex = null;
            Options = null;
            _settings = null;
            _settingsConfig = null;
        }
    }
}
