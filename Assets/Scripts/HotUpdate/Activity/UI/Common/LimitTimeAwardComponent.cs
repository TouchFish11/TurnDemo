using System;
using Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Activity.UI.Common
{
    /// <summary>
    /// 限时奖励UI组件
    /// </summary>
    public class LimitTimeAwardComponent : UIBehaviourBase
    {
        [Inject] private Image imgBk;
        [Inject] private TextMeshProUGUI txtAward;
        [Inject] private Button btnLimitedTimeAward;
        
        public event Action OnClickAward;

        /// <summary>
        /// 设置限时奖励背景
        /// </summary>
        /// <param name="bk"></param>
        public void SetBackGround(Sprite bk)
        {
            imgBk.sprite = bk;
        }
        
        /// <summary>
        /// 设置提示文本
        /// </summary>
        /// <param name="txtTip"></param>
        public void SetTipTitle(out TextMeshProUGUI txtTip)
        {
            txtTip = txtAward;
        }

        protected override void OnButtonClick(string btnName)
        {
            if (btnName == nameof(btnLimitedTimeAward))
            {
                OnClickAward?.Invoke();
            }
        }
    }
}
