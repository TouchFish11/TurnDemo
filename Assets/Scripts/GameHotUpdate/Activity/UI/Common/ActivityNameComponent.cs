using Core.UI;
using TMPro;

namespace GameHotUpdate.Activity.UI.Common
{
    /// <summary>
    /// 活动名称UI组件
    /// </summary>
    public class ActivityNameComponent : BaseUIBehaviour
    {
        [Inject] private TextMeshProUGUI txtActivityName;

        /// <summary>
        /// 设置活动名称样式
        /// </summary>
        /// <param name="txtActivityName"></param>
        public void SetTitle(out TextMeshProUGUI txtActivityName)
        {
            txtActivityName = this.txtActivityName;
        }
    }
}
