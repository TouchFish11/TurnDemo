using Core;

namespace HotUpdate.Core.Main.Settings.ViewModel
{
    /// <summary>
    /// 滑动条设置VM
    /// </summary>
    public class SettingSliderViewModel
    {
        private GameSettings _settings;
        public ReactiveProperty<string> ProgressText { get; } =  new();
        public ReactiveProperty<float> ProgressSlider { get; } = new();

        public SettingSliderViewModel(GameSettings gameSettings)
        {
            ProgressSlider.OnValueChanged += volumeValue => gameSettings.Volume = volumeValue;
            gameSettings.OnDataChanged += settings =>
            {
                ProgressText.Value = $"{settings.Volume * 10}";
                ProgressSlider.Value = settings.Volume;     // UI导致的变化内部会有相等性判断进行筛选，而这行代码是为了能响应数据本身的变化
            };
            _settings = gameSettings;
        }
    }
}
