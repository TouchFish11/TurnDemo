using Core.UI;
using HotUpdate.Activity.UI.Base;
using TMPro;

namespace HotUpdate.Activity.UI.Common
{
    /// <summary>
    /// 活动描述UI组件
    /// </summary>
    public class ActivityDescritionComponent : ActivityUIComponent
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

        protected override void OnInit()
        {

        }
    }
}
