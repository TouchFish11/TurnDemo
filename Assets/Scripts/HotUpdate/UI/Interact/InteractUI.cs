using Core.Log;
using Core.UI;
using TMPro;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Interact.UI
{
    /// <summary>
    /// 交互UI
    /// </summary>
    public class InteractUI : UIBehaviourBase
    {
        [InjectUI] private TextMeshProUGUI txtInteractTip;

        protected override void OnButtonClick(string btnName)
        {
            Logger.LogDebug(ELogTags.Interact, "交互按钮点击");
        }

        public void Init(string text)
        {
            txtInteractTip.text = text;
        }

        public GameObject GameObject => this.gameObject;
    }
}
