using System.Collections.Generic;
using Core.UI;
using HotUpdate.UI.Activity.Base;
using HotUpdate.UI.Item;
using UnityEngine.UI;

namespace HotUpdate.UI.Activity.Common
{
    /// <summary>
    /// 活动奖励预览组件
    /// </summary>
    public class AwardPreviewComponent : ActivityUIComponent 
    {
        [InjectUI] private ScrollRect svAward;

        protected override void OnInit()
        {

        }
        
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

        protected override void OnDestroy()
        {
            svAward = null;
            base.OnDestroy();
        }
    }
}
