using Core.UI;
using TMPro;

namespace HotUpdate.UI.Interact
{
    /// <summary>
    /// 交互UI
    /// </summary>
    public class InteractUI : UIBehaviourBase, ILogicView<InteractUI, InteractLogic>
    {
        [InjectUI] public TextMeshProUGUI txtInteractTip;

        private InteractLogic _logic;
        
        public void Init(InteractLogic logic)
        {
            _logic = logic;
            txtInteractTip.text = _logic.InteractTipText;
        }
        
        protected override void OnButtonClick(string btnName)
        {
            _logic.TriggerInteract();
        }
    }
}
