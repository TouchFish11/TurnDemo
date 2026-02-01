using Core.UI;
using Core.UI.MVC;
using UnityEngine;
using UnityEngine.UI;

namespace GameHotUpdate.UI.Global
{
    /// <summary>
    /// ȫ����Ϣ����
    /// </summary>
    public class GlobalMessageView : UIView
    {
        [Inject] private ScrollRect svMsg;

        public Transform MessageContainer => svMsg.content;
    }
}
