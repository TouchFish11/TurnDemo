using Core.UI;
using TMPro;
using UnityEngine.UI;

namespace GameHotUpdate.Tip.UI.Update
{
    /// <summary>
    /// 更新提示界面
    /// </summary>
    public class UpdateTipView : TipView
    {
        [Inject] public TextMeshProUGUI txtUpdateTip;
        [Inject] public Button btnSure;
        [Inject] public TextMeshProUGUI txtTip;

        /// <summary>
        /// 设置更新提示文本
        /// </summary>
        /// <param name="tip"></param>
        public void SetUpdateTip(string tip)
        {
            txtUpdateTip.text = tip;
        }
        
        /// <summary>
        /// 设置提示信息
        /// </summary>
        /// <param name="isActive">false则隐藏，tip忽略；true则显示tip的内容</param>
        /// <param name="tip">显示的文本</param>
        public void SetTipActive(bool isActive, string tip = "")
        {
            if (isActive)
            {
                txtTip.gameObject.SetActive(true);   
                txtTip.text = tip;
            }
            else
            {
                txtTip.gameObject.SetActive(false);
                txtTip.text = string.Empty;
            }
        }
    }
}
