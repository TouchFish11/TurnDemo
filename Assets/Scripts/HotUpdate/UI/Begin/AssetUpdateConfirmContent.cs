using Core.UI;
using HotUpdate.Base;
using HotUpdate.Base.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI.Begin
{
    public class AssetUpdateConfirmContent : UIBehaviourBase, IConfirmContent
    {
        [InjectUI] private TextMeshProUGUI txtUpdateTip;
        [InjectUI] private TextMeshProUGUI txtAuxiliaryTip;
        [InjectUI] private Button btnSure;
        
        private ConfirmData _confirmData;
        
        [InjectUI(1)] private RectTransform AuxiliaryTipBox { get; set; }
        
        public void DrawContent(ConfirmData confirmData)
        {
            txtUpdateTip.text = confirmData.ConfirmMessage;
            
            AuxiliaryTipBox.gameObject.SetActive(true);
            txtAuxiliaryTip.text = $"{confirmData.ContentData}";
            
            _confirmData = confirmData;
        }

        public void ClearContent()
        {
            _confirmData = null;
        }

        protected override void OnButtonClick(string btnName)
        {
            if (btnName == nameof(btnSure))
            {
                _confirmData.OnConfirm?.Invoke();
            }
        }
    }
}
