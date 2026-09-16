using Core.DI;
using HotUpdate.Base.Data;

namespace HotUpdate.UI.Settings.Handlers
{
    /// <summary>
    /// 「重置所有设置」处理器：把所有设置值恢复为配置里的默认值。
    /// 只负责清数据；UI 的刷新（让滑块/下拉回到默认显示）由 SettingsController 在调用后重建条目完成。
    /// </summary>
    public class ResetSettingsHandler : IButtonSettingHandler
    {
        [Inject] private IMainDataProvider mainDataProvider;

        public void Execute()
        {
            mainDataProvider.GameSettings.Reset(mainDataProvider.GameSettingsConfig);
        }
    }
}
