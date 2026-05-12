namespace HotUpdate.Base.Main.Settings.ViewModel
{
    /// <summary>
    /// 音效开关下拉菜单ViewModel
    /// </summary>
    public class SFXOpenDropdownViewModel : SettingDropdownViewModel
    {
        public SFXOpenDropdownViewModel(GameSettings settings, GameSettingsConfig settingsConfig) : base(settings, settingsConfig)
        {
            Options = settingsConfig.sfxOpts.ConvertAll(i => i.ToString());
            // 监听变化事件
            OptionIndex.Subscribe(optionIndex => settings[ESettingType.SFXOpen] = optionIndex);
            settings.OnDataChanged += OnSettingsChanged;
        }

        protected override void OnSettingsChanged(GameSettings settings)
        {
            RefleshUI();
        }

        public override void RefleshUI()
        {
            OptionIndex.Value = (int)_settings[ESettingType.SFXOpen];
        }
    }
}
