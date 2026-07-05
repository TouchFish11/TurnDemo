using System.Collections.Generic;
using Core.UI;
using Core.UI.ViewController;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI.Settings.UI
{
    /// <summary>
    /// 设置界面
    /// </summary>
    public class SettingsView : UIView
    {
        [InjectUI] public ScrollRect svOpts;
        [InjectUI] public ScrollRect svEntrys;
        [InjectUI] public Button btnClose;

        private List<SettingOpt> _settingOpts =  new();
        private List<ISettingsEntry> _settingsEntries = new();
        
        public Transform Opts => svOpts.content;
        
        public Transform Entrys => svEntrys.content;
    }
}
