using System;
using System.Collections.Generic;
using Core.UI;
using HotUpdate.Base.Settings;
using HotUpdate.Common.Config.Settings;

namespace HotUpdate.UI.Settings.ViewModel
{
    /// <summary>
    /// 下拉列表设置ViewModel
    /// </summary>
    public class SettingDropdownViewModel : IDisposable
    {
        /// <summary>
        /// 选中索引
        /// </summary>
        public ReactiveProperty<int> OptionIndex { get; protected set; }
        
        /// <summary>
        /// 选项列表
        /// </summary>
        public List<string> Options { get; protected set; }
        
        protected SettingDropdownViewModel(GameSettings settings, GameSettingsConfig settingsConfig, ESettingType settingType)
        {
            switch (settingType)
            {
                case ESettingType.VolumeOpen:
                    Options = settingsConfig.volumeOpts.ConvertAll(i => i.ToString());
                    break;
                case ESettingType.SFXOpen:
                    Options = settingsConfig.sfxOpts.ConvertAll(i => i.ToString());
                    break;
                case ESettingType.TypeWriter:
                    Options = settingsConfig.typeWriterOpts.ConvertAll(i => i.ToString());
                    break;
                case ESettingType.TargetFrameRateIndex:
                    Options = settingsConfig.framerates.ConvertAll(i => i.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(settingType), settingType, null);
            }

            OptionIndex = new ReactiveProperty<int>((int)settings[settingType]);
        }

        public void Dispose()
        {
            OptionIndex = null;
            Options = null;
        }
    }
}
