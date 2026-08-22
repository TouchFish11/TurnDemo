using Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI.Battle.ActionLine
{
    /// <summary>
    /// 等待行动UI
    /// </summary>
    public class WaitingActUI : UIBehaviourBase, ILogicView<WaitingActUI, WaitingActLogic>
    {
        [InjectUI] private Image imgIcon;

        private WaitingActLogic _waitingActLogic;
        
        public void Init(WaitingActLogic logic)
        {
            _waitingActLogic = logic;
            SetIcon(_waitingActLogic.Icon);
        }
        
        private void SetIcon(Sprite icon)
        {
            imgIcon.sprite = icon;
        }
    }
}
