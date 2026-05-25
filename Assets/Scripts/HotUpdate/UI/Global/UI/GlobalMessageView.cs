using Core.UI;
using Core.UI.ViewController;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Game.Main.Global.UI
{
    /// <summary>
    /// ȫ����Ϣ����
    /// </summary>
    public class GlobalMessageView : UIView
    {
        [InjectUI] private ScrollRect svMsg;

        public Transform MessageContainer => svMsg.content;
    }
}
