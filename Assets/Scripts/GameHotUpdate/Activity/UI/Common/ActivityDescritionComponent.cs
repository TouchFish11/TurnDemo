using Core.UI;
using TMPro;

namespace GameHotUpdate.Activity.UI.Common
{
    /// <summary>
    /// 活动描述UI组件
    /// </summary>
    public class ActivityDescritionComponent : BaseUIBehaviour
    {
        [Inject] private TextMeshProUGUI txtActivityDescrition;
        
        /// <summary>
        /// 设置活动描述样式
        /// </summary>
        /// <param name="txtActivityDescrition"></param>
        public void SetActivityDescrition(out TextMeshProUGUI txtActivityDescrition)
        {
            txtActivityDescrition = this.txtActivityDescrition;
        }
    }
}
