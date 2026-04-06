namespace HotUpdate.Core.Main.Settings.ViewModel
{
    /// <summary>
    /// 音效音量滑动条ViewModel
    /// </summary>
    public class SFXSliderViewModel : SettingSliderViewModel
    {
        public SFXSliderViewModel(GameSettings gameSettings) : base(gameSettings)
        {
            ProgressSlider.Subscribe(sfxValue => gameSettings[ESettingType.SFXValue] = sfxValue / SLIDER_MULTIPLIER);
            gameSettings.OnDataChanged += OnSettingsChange;
        }

        protected override void OnSettingsChange(GameSettings settings)
        {
            RefleshUI();
        }

        public override void RefleshUI()
        {
            // 音效音量
            ProgressSlider.Value = (float)_settings[ESettingType.SFXValue] * SLIDER_MULTIPLIER;
            // UI导致的变化内部会有相等性判断进行筛选，而这行代码是为了能响应数据本身的变化
            ProgressText.Value = $"{(float)_settings[ESettingType.SFXValue] * SLIDER_MULTIPLIER}";
        }
    }
}
