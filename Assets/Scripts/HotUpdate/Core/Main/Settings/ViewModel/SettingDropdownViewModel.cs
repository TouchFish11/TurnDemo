using System;
using System.Collections.Generic;
using Core;

namespace HotUpdate.Core.Main.Settings.ViewModel
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
        /// 更新
        /// </summary>
        public abstract void Update();

        public virtual void Dispose()
        {
            OptionIndex = null;
            Options = null;
            _settings = null;
            _settingsConfig = null;
        }
    }
}
