using System;
using Core.DI;
using HotUpdate.Base.Settings;
using HotUpdate.Common.Config.Settings;

namespace HotUpdate.UI.Settings.ViewModel
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
            return DIContainer.Create<SettingSliderViewModel>(parameterValues: new object[] { settings, type });
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
            return DIContainer.Create<SettingDropdownViewModel>(parameterValues: new object[] { settings, settingsConfig, type });
        }
    }
}
