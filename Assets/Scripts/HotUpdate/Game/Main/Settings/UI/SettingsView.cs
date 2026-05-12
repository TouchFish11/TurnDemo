using System.Collections.Generic;
using Core.DI;
using Core.UI.ViewController;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Game.Main.Settings.UI
{
    /// <summary>
    /// 设置界面
    /// </summary>
    public class SettingsView : UIView
    {
        [Inject] public ScrollRect svOpts;
        [Inject] public ScrollRect svEntrys;
        [Inject] public Button btnClose;

        private List<SettingOpt> _settingOpts =  new();
        private List<ISettingsEntry> _settingsEntries = new();
        
        public Transform Opts => svOpts.content;
        
        public Transform Entrys => svEntrys.content;
    }
}
