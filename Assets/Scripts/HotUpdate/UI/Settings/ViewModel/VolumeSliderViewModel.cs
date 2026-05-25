using HotUpdate.Base.Settings;

namespace HotUpdate.UI.Settings.ViewModel
{
    /// <summary>
    /// 音乐音量滑动条ViewModel
    /// </summary>
    public class VolumeSliderViewModel : SettingSliderViewModel
    {
        public VolumeSliderViewModel(GameSettings gameSettings) : base(gameSettings)
        {
            ProgressSlider.Subscribe(volumeValue => gameSettings[ESettingType.VolumeValue] = volumeValue / SLIDER_MULTIPLIER);
            gameSettings.OnDataChanged += OnSettingsChange;
        }
        
        protected override void OnSettingsChange(GameSettings settings)
        {
            RefleshUI();
        }

        public override void RefleshUI()
        {
            // 音乐音量
            ProgressSlider.Value = (float)_settings[ESettingType.VolumeValue] * SLIDER_MULTIPLIER;
            // UI导致的变化内部会有相等性判断进行筛选，而这行代码是为了能响应数据本身的变化
            ProgressText.Value = $"{(float)_settings[ESettingType.VolumeValue] * SLIDER_MULTIPLIER}";
        }
    }
}
