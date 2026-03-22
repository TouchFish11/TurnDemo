using System.Collections.Generic;
using Core.Global;
using Core.UI.MVC;

namespace HotUpdate.Main.Settings
{
    /// <summary>
    /// 设置界面数据
    /// </summary>
    public class SettingsModel : UIModel
    {
        private List<SettingOpt> _settingOpts =  new();
        private List<ISettingsEntry> _settingsEntries = new();
        private GameSettings _gameSettings;
        
        
        
    }
}
