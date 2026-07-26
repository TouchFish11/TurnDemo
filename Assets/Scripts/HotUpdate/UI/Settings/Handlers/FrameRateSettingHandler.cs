using Core.DI;
using HotUpdate.Base.Data;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Settings;

namespace HotUpdate.UI.Settings.Handlers
{
    public class FrameRateSettingHandler : DropdownSettingHandler
    {
        [Inject] private IMainDataProvider mainDataProvider;

        public override void Execute(int optionIndex)
        {
            // 设置帧率
            SettingsService.SetFrameRate(mainDataProvider.GameSettingsConfig.framerates[optionIndex]);
        }
    }
}
