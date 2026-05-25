using Core.DI;
using HotUpdate.Common.Item.UI;
using HotUpdate.UI.Activity.Base;
using UnityEngine.UI;

namespace HotUpdate.Activity.UI.Common
{
    /// <summary>
    /// 活动奖励预览组件
    /// </summary>
    public class AwardPreviewComponent : ActivityUIComponent 
    {
        [Inject] private ScrollRect svAward;

        /// <summary>
        /// 设置奖励显示
        /// </summary>
        /// <param name="awards"></param>
        public void SetAwards(params ItemGrid[] awards)
        {
            foreach (var award in awards)
            {
                SetAward(award);
            }
        }
        
        /// <summary>
        /// 设置奖励显示
        /// </summary>
        /// <param name="award"></param>
        public void SetAward(ItemGrid award)
        {
            award.gameObject.transform.SetParent(svAward.content, false);
        }

        protected override void OnInit()
        {

        }
    }
}
