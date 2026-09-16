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

        protected SettingSliderViewModel(GameSettings gameSettings, SettingDefinition definition)
        {
            Progress = new ReactiveProperty<float>(gameSettings.GetFloat(definition.Type));
        }

        public void Dispose()
        {
            Progress.Dispose();
            Progress = null;
        }
    }
}
