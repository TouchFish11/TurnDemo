using System.Collections.Generic;
using Core.UI.MVC;
using HotUpdate.Core.Main.Settings;

namespace HotUpdate.Main.Settings.UI
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
