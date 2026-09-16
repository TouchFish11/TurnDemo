using Core.DI;
using HotUpdate.Base.Data;
using HotUpdate.Base.Settings;

namespace HotUpdate.UI.Settings.Handlers
{
    public class FrameRateSettingHandler : DropdownSettingHandler
    {
        [Inject] private IMainDataProvider mainDataProvider;

        public override void Execute(int optionIndex)
        {
            var definition = mainDataProvider.GameSettingsConfig.Settings.Find(d => d.Type == ESettingType.TargetFrameRateIndex);
            SettingsService.SetFrameRate(definition.Options[optionIndex].Value);
        }
    }
}
