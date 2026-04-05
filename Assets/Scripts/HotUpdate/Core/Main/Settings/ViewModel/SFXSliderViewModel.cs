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
            gameSettings.OnDataChanged += settings =>
            {
                ProgressText.Value = $"{(float)settings[ESettingType.SFXValue] * SLIDER_MULTIPLIER}";
                ProgressSlider.Value = (float)settings[ESettingType.SFXValue] * SLIDER_MULTIPLIER;     // UI导致的变化内部会有相等性判断进行筛选，而这行代码是为了能响应数据本身的变化
            };
        }

        public override void Update()
        {
            // 音效音量
            ProgressSlider.Value = (float)_settings[ESettingType.SFXValue] * SLIDER_MULTIPLIER;
            ProgressText.Value = $"{(float)_settings[ESettingType.SFXValue] * SLIDER_MULTIPLIER}";
        }
    }
}
