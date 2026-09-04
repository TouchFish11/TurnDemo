using Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI.Battle.ActionLine
{
    /// <summary>
    /// 状态描述提示UI
    /// </summary>
    public class StatusTipUI : UIBehaviourBase
    {
        [InjectUI] public Image imgIcon;
        [InjectUI] public TextMeshProUGUI txtBuffName;
        [InjectUI] public TextMeshProUGUI txtRemainRound;
        [InjectUI] public TextMeshProUGUI txtDetailTip;

        public void Init(Sprite icon, string buffName, int remainRound, string detailTip)
        {
            imgIcon.sprite = icon;
            txtBuffName.text = buffName;
            txtRemainRound.text = remainRound.ToString();
            txtDetailTip.text = detailTip;
        }
    }
}
