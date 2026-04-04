using System.Collections.Generic;
using Core;
using Unity.VisualScripting;

namespace HotUpdate.Core.Main.Settings.ViewModel
{
    /// <summary>
    /// 下拉列表设置VM
    /// </summary>
    public class SettingDrowdownViewModel
    {
        private readonly GameSettings _settings;
        private readonly GameSettingsConfig _settingsConfig;
        public ReactiveProperty<int> OptionIndex { get; } = new();
        public ReactiveProperty<List<string>> Options { get; } = new();
        
        public SettingDrowdownViewModel(GameSettings settings, GameSettingsConfig settingsConfig)
        {
            // 更新数据
            Update();
            // 监听变化事件
            OptionIndex.OnValueChanged += optionIndex => settings.TargetFrameRateIndex = optionIndex;
            settings.OnDataChanged += settings =>
            {
                OptionIndex.Value = settings.TargetFrameRateIndex;
                Options.Value ??= settingsConfig.framerates.ConvertTo<List<string>>();
            };
            _settings = settings;
            _settingsConfig = settingsConfig;
        }
        
        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            // 先添加选项
            Options.Value = _settingsConfig.framerates.ConvertTo<List<string>>();
            OptionIndex.Value = _settings.TargetFrameRateIndex;
        }
    }
}
