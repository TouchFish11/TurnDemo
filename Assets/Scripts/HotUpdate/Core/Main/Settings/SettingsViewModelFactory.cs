using System;
using HotUpdate.Core.Main.Settings.ViewModel;

namespace HotUpdate.Core.Main.Settings
{
    /// <summary>
    /// 设置UI的ViewModel工厂
    /// </summary>
    public class SettingsViewModelFactory
    {
        /// <summary>
        /// 创建滑动条ViewModel
        /// </summary>
        /// <param name="type"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static SettingSliderViewModel CreateSliderViewModel(ESettingType type, GameSettings settings)
        {
            return type switch
            {
                ESettingType.VolumeValue => new VolumeSliderViewModel(settings),
                ESettingType.SFXValue => new SFXSliderViewModel(settings),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
        
        /// <summary>
        /// 创建下拉菜单ViewModel
        /// </summary>
        /// <param name="type"></param>
        /// <param name="settings"></param>
        /// <param name="settingsConfig"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static SettingDropdownViewModel CreateDropdownViewModel(ESettingType type, GameSettings settings, GameSettingsConfig settingsConfig)
        {
            return type switch
            {
                ESettingType.VolumeOpen => new VolumeOpenDropdownViewModel(settings, settingsConfig),
                ESettingType.SFXOpen => new SFXOpenDropdownViewModel(settings, settingsConfig),
                ESettingType.TypeWriter => new TypeWriterDropdownViewModel(settings, settingsConfig),
                ESettingType.TargetFrameRateIndex => new FrameRateDropdownViewModel(settings, settingsConfig),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}
