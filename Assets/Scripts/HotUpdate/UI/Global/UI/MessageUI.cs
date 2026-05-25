using Core.UI;
using TMPro;

namespace HotUpdate.UI.Global.UI
{
    /// <summary>
    /// 消息UI
    /// </summary>
    public class MessageUI : UIBehaviourBase
    {
        [InjectUI] private TextMeshProUGUI txtMsg;

        /// <summary>
        /// 初始化消息
        /// </summary>
        /// <param name="msg"></param>
        public void InitMessage(string msg)
        {
            txtMsg.text = msg;
        }
    }
}
