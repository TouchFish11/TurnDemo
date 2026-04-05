using Core;

namespace HotUpdate.Core.Main.Settings.ViewModel
{
    /// <summary>
    /// 滑动条设置ViewModel
    /// </summary>
    public abstract class SettingSliderViewModel
    {
        protected readonly GameSettings _settings;
        protected const int SLIDER_MULTIPLIER = 10;
        public ReactiveProperty<string> ProgressText { get; } =  new();
        public ReactiveProperty<float> ProgressSlider { get; } = new();

        protected SettingSliderViewModel(GameSettings gameSettings)
        {
            _settings = gameSettings;
        }

        /// <summary>
        /// 更新
        /// </summary>
        public abstract void Update();
    }
}
