using System;
using Core.UI;
using GameHotUpdate.Activity.UI.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameHotUpdate.Activity.UI.Common
{
    /// <summary>
    /// 参与按钮UI组件
    /// </summary>
    public class ActivityJoinComponent : ActivityUIComponent
    {
        [Inject] private Button btnJoin;
        [Inject] private TextMeshProUGUI txtJoin;
        // 参与按钮图片
        private Image imgBtnJoin;
        
        public event Action OnClickJoin;

        protected override void Awake()
        {
            base.Awake();
            imgBtnJoin = btnJoin.image;
        }
        
        protected override void OnInit()
        {

        }
        
        /// <summary>
        /// 设置参与按钮的图片
        /// </summary>
        /// <param name="sprite"></param>
        public void SetImageAtBtnJoin(Sprite sprite)
        {
            imgBtnJoin.sprite = sprite;
        }
        
        /// <summary>
        /// 设置参与按钮的本文样式
        /// </summary>
        /// <param name="txtJoin"></param>
        public void SetTitle(out TextMeshProUGUI txtJoin)
        {
            txtJoin = this.txtJoin;
        }

        protected override void OnButtonClick(string btnName)
        {
            if (btnName == nameof(btnJoin))
            {
                OnClickJoin?.Invoke();
            }
        }
    }
}
