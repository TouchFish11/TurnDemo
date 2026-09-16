using Core.DI;
using HotUpdate.Base.Settings;
using HotUpdate.UI.Settings.Handlers;

namespace HotUpdate.UI.Settings.ViewModel
{
    /// <summary>
    /// 设置 UI 的 ViewModel 工厂，根据定义创建对应的 ViewModel
    /// </summary>
    public class SettingsViewModelFactory
    {
        public static SettingSliderViewModel CreateSliderViewModel(GameSettings settings, SettingDefinition definition)
        {
            return DIContainer.Create<SettingSliderViewModel>(parameterValues: new object[] { settings, definition });
        }

        public static SettingDropdownViewModel CreateDropdownViewModel(GameSettings settings, SettingDefinition definition)
        {
            // 分辨率选项依赖当前显示器，运行期从 Screen.resolutions 动态生成；其余用 SO 里静态配置的选项
            var options = definition.Options;
            if (definition.Type == ESettingType.Resolution)
                options = ResolutionHandler.GetOptions();

            return DIContainer.Create<SettingDropdownViewModel>(parameterValues: new object[] { settings, definition, options });
        }
    }
}
