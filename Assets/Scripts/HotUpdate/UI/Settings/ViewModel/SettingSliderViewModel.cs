using System;
using Core.UI;
using HotUpdate.Base.Settings;

namespace HotUpdate.UI.Settings.ViewModel
{
    /// <summary>
    /// 滑动条设置ViewModel
    /// </summary>
    public class SettingSliderViewModel : IDisposable
    {
        /// <summary>
        /// 滑动条进度
        /// </summary>
        public ReactiveProperty<float> Progress { get; protected set; }

        protected SettingSliderViewModel(GameSettings gameSettings, ESettingType settingType)
        {
            Progress = new ReactiveProperty<float>((float)gameSettings[settingType] * SettingsUtil.SLIDER_MULTIPLIER);
        }
        
        public void Dispose()
        {
            Progress.Dispose();
            Progress = null;
        }
    }
}
