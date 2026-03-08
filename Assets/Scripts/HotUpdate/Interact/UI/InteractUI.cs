using Core.Log;
using Core.UI;
using HotUpdate.Core.Interact;
using TMPro;
using UnityEngine;

namespace HotUpdate.Interact.UI
{
    /// <summary>
    /// 交互UI
    /// </summary>
    public class InteractUI : UIBehaviourBase, IInteractUI
    {
        [Inject] private TextMeshProUGUI txtInteractTip;

        protected override void OnButtonClick(string btnName)
        {
            LogManager.Log("交互按钮点击");
        }

        public void Init(string text)
        {
            txtInteractTip.text = text;
        }

        public GameObject GameObject => this.gameObject;
    }
}
