using Core.UI;
using Core.UI.MVC;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Main.Settings
{
    /// <summary>
    /// 设置界面
    /// </summary>
    public class SettingsView : UIView
    {
        [Inject] private ScrollRect svOpts;
        [Inject] private ScrollRect svEntrys;

        public Transform Opts => svOpts.content;
        
        public Transform Entrys => svEntrys.content;
    }
}
