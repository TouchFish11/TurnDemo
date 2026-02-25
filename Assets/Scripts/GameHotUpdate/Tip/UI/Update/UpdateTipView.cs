using Core.Service;
using Core.UI;
using TMPro;
using UnityEngine.UI;

namespace GameHotUpdate.Tip.UI.Update
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateTipView : TipView
    {
        [Inject] private TextMeshProUGUI txtTip;
        [Inject] public Button btnSure;

        public void SetTip(string tip)
        {
            txtTip.text = tip;
        }
    }
}
