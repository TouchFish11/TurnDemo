using HotUpdate.Base.Settings;

namespace HotUpdate.UI.Settings.ViewModel
{
    /// <summary>
    /// 音量开关下拉菜单ViewModel
    /// </summary>
    public class VolumeOpenDropdownViewModel : SettingDropdownViewModel
    {
        public VolumeOpenDropdownViewModel(GameSettings settings, GameSettingsConfig settingsConfig) : base(settings, settingsConfig)
        {
            Options = settingsConfig.volumeOpts.ConvertAll(i => i.ToString());
            // 监听变化事件
            OptionIndex.Subscribe(optionIndex => settings[ESettingType.VolumeOpen] = optionIndex);
            settings.OnDataChanged += OnSettingsChanged;
        }

        protected override void OnSettingsChanged(GameSettings settings)
        {
            RefleshUI();
        }

        public override void RefleshUI()
        {
            OptionIndex.Value = (int)_settings[ESettingType.VolumeOpen];
        }
    }
}
