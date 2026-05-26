using HotUpdate.Base.Settings;
using HotUpdate.Common.Config.Settings;

namespace HotUpdate.UI.Settings.ViewModel
{
    /// <summary>
    /// 打字机效果下拉菜单ViewModel
    /// </summary>
    public class TypeWriterDropdownViewModel : SettingDropdownViewModel
    {
        public TypeWriterDropdownViewModel(GameSettings settings, GameSettingsConfig settingsConfig) : base(settings, settingsConfig)
        {
            Options = settingsConfig.typeWriterOpts.ConvertAll(i => i.ToString());
            // 监听变化事件
            OptionIndex.Subscribe(optionIndex => settings[ESettingType.TypeWriter] = optionIndex);
            settings.OnDataChanged += OnSettingsChanged;
        }

        protected override void OnSettingsChanged(GameSettings settings)
        {
            OptionIndex.Value = (int)settings[ESettingType.TypeWriter];
        }

        public override void RefleshUI()
        {
            OptionIndex.Value = (int)_settings[ESettingType.TypeWriter];
        }
    }
}
