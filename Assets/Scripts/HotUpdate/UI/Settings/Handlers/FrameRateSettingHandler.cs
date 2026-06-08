using Core.DI;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Settings;

namespace HotUpdate.UI.Settings.Handlers
{
    public class FrameRateSettingHandler : DropdownSettingHandler
    {
        [Inject] private IMainDataManager _mainDataManager;

        public override void Execute(int optionIndex)
        {
            // 设置帧率
            SettingsService.SetFrameRate(_mainDataManager.GameSettingsConfig.framerates[optionIndex]);
        }
    }
}
