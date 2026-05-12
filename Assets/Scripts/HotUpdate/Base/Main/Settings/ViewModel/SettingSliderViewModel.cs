using System;
using Core.UI;

namespace HotUpdate.Base.Main.Settings.ViewModel
{
    /// <summary>
    /// 滑动条设置ViewModel
    /// </summary>
    public abstract class SettingSliderViewModel : IDisposable
    {
        protected GameSettings _settings;
        protected const int SLIDER_MULTIPLIER = 10;
        
        public ReactiveProperty<string> ProgressText { get; private set; } = new();
        public ReactiveProperty<float> ProgressSlider { get; private set; } = new();

        protected SettingSliderViewModel(GameSettings gameSettings)
        {
            _settings = gameSettings;
        }

        protected abstract void OnSettingsChange(GameSettings settings);

        /// <summary>
        /// 刷新UI显示，UI初始化时需主动调用该方法，拉取数据来显示
        /// </summary>
        public abstract void RefleshUI();

        public void Dispose()
        {
            _settings.OnDataChanged -= OnSettingsChange;
            ProgressSlider = null;
            ProgressText = null;
            _settings = null;
        }
    }
}
