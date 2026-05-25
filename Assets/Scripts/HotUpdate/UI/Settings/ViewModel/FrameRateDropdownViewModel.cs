using HotUpdate.Base.Settings;

namespace HotUpdate.UI.Settings.ViewModel
{
    /// <summary>
    /// 帧率下拉菜单ViewModel
    /// </summary>
    public class FrameRateDropdownViewModel : SettingDropdownViewModel
    {
        public FrameRateDropdownViewModel(GameSettings settings, GameSettingsConfig settingsConfig) : base(settings, settingsConfig)
        {
            Options = _settingsConfig.framerates.ConvertAll(i => i.ToString());
            // 监听变化事件
            OptionIndex.Subscribe(optionIndex =>
            {
                settings[ESettingType.TargetFrameRateIndex] = optionIndex;
                // 设置帧率
                SettingsService.SetFrameRate(settingsConfig.framerates[optionIndex]);
            });
            settings.OnDataChanged += OnSettingsChanged;
        }

        protected override void OnSettingsChanged(GameSettings settings)
        {
            RefleshUI();
        }

        public override void RefleshUI()
        {
            OptionIndex.Value = (int)_settings[ESettingType.TargetFrameRateIndex];
        }
    }
}
