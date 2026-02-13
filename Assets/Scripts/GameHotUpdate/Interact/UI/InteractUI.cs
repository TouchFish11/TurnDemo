using Core.Log;
using Core.UI;
using TMPro;

namespace GameHotUpdate.Interact.UI
{
    /// <summary>
    /// ����UI
    /// </summary>
    public class InteractUI : UIBehaviourBase
    {
        [Inject] private TextMeshProUGUI txtInteractTip;
        
        protected override void OnButtonClick(string btnName)
        {
            LogManager.Log($"��ť���");
        }

        public void Init(string text)
        {
            txtInteractTip.text = text;
        }
    }
}
