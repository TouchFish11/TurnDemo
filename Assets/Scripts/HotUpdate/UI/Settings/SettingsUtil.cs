using System;
using HotUpdate.Base.Settings;
using HotUpdate.Common.Config.Settings;

namespace HotUpdate.UI.Settings
{
    /// <summary>
    /// 设置工具类
    /// </summary>
    public static class SettingsUtil
    {
        public const int SLIDER_MULTIPLIER = 10;
        
        /// <summary>
        /// 设置项UI类型转文本名称
        /// </summary>
        /// <param name="type"></param>
        /// <param name="settingsConfig"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string SettingTypeToStr(ESettingType type, GameSettingsConfig settingsConfig)
        {
            return type switch
            {
                ESettingType.VolumeValue => settingsConfig.volumeItemName,
                ESettingType.SFXValue => settingsConfig.sfxItemName,
                ESettingType.VolumeOpen => settingsConfig.volumeOpenItemName,
                ESettingType.SFXOpen => settingsConfig.sfxOpenItemName,
                ESettingType.TypeWriter => settingsConfig.typeWriterItemName,
                ESettingType.TargetFrameRateIndex => settingsConfig.frameRateItemName,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}
