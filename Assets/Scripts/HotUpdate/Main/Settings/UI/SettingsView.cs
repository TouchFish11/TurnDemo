using Core.UI;
using Core.UI.MVC;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Main.Settings.UI
{
    /// <summary>
    /// 设置界面
    /// </summary>
    public class SettingsView : UIView
    {
        [Inject] public ScrollRect svOpts;
        [Inject] public ScrollRect svEntrys;
        [Inject] public Button btnClose;

        public Transform Opts => svOpts.content;
        
        public Transform Entrys => svEntrys.content;
    }
}
