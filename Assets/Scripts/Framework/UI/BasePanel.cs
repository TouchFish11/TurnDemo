using UnityEngine.Events;
using UnityEngine;
using Unity.VisualScripting;

namespace Framework
{
    /// <summary>
    /// 面板基类
    /// </summary>
    public abstract class BasePanel : BaseUI
    {
        //画布组
        protected CanvasGroup canvasGroup;
        //透明度变化率
        protected float alphaSpeed = 1f;
        //是否隐藏
        private bool _isHide;
        //隐藏回调
        private UnityAction _hideCallBack;

        protected override void Awake()
        {
            base.Awake();

            this.canvasGroup = this.GetComponent<CanvasGroup>();
            if (this.canvasGroup == null)
                this.canvasGroup = this.AddComponent<CanvasGroup>();
        }

        /// <summary>
        /// 显示
        /// </summary>
        public virtual void Show()
        {
            this._isHide = false;
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        /// <param name="hideCallBack">隐藏结束回调</param>
        public virtual void Hide(UnityAction hideCallBack = null)
        {
            this._isHide = true;
            this._hideCallBack = hideCallBack;
        }

        protected virtual void Update()
        {
            //逐渐隐藏
            if (this._isHide && this.canvasGroup.alpha > 0)
            {
                this.canvasGroup.alpha -= Time.unscaledDeltaTime * this.alphaSpeed;
                if (this.canvasGroup.alpha < 0)
                {
                    this.canvasGroup.alpha = 0;
                    this._hideCallBack?.Invoke();
                    this._hideCallBack = null;
                }
            }
            //逐渐显示
            else if (!this._isHide && this.canvasGroup.alpha < 1)
            {
                this.canvasGroup.alpha += Time.unscaledDeltaTime * this.alphaSpeed;
                if (this.canvasGroup.alpha > 1)
                {
                    this.canvasGroup.alpha = 1;
                }
            }
        }
    }
}
