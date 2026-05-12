using Core.DI;
using Core.Time;
using Core.UI;
using TMPro;
using UnityEngine;

namespace HotUpdate.Game.Main.Global.UI
{
    /// <summary>
    /// 消息UI
    /// </summary>
    public class MessageUI : UIBehaviourBase
    {
        [Inject] private TextMeshProUGUI txtMsg;
        // 显示时间
        [SerializeField] private float duration = 2.5f;

        /// <summary>
        /// 初始化消息
        /// </summary>
        /// <param name="msg"></param>
        public void InitMessage(string msg)
        {
            txtMsg.text = msg;
            DIContainer.GetInstance<ITimerManager>().CreateTimer(false, (int)(duration * 1000), CollectObj);
        }

        private void CollectObj()
        {
            DIContainer.GetInstance<IPrefabLoader>().CollectAsset(this.gameObject);
        }
    }
}
