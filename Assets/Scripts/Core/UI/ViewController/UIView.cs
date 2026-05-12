using System;
using UnityEngine;

namespace Core.UI.ViewController
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIView : UIBehaviourBase, IuiView
    {
        // 画布组
        protected CanvasGroup canvasGroup;
        // 透明度变化率
        protected const float alphaSpeed = 1f;
        // 是否隐藏
        private bool _isHide;
        // 隐藏回调
        private Action _hideCallBack;
        
        public GameObject ViewObj { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            canvasGroup = GetComponent<CanvasGroup>();
            ViewObj = gameObject;
        }

        protected virtual void Update()
        {
            switch (_isHide)
            {
                // 逐渐隐藏
                case true when canvasGroup.alpha > 0:
                {
                    canvasGroup.alpha -= UnityEngine.Time.unscaledDeltaTime * alphaSpeed;
                    if (canvasGroup.alpha < 0)
                    {
                        canvasGroup.alpha = 0;
                        _hideCallBack?.Invoke();
                        _hideCallBack = null;
                    }

                    break;
                }
                // 逐渐显示
                case false when canvasGroup.alpha < 1:
                {
                    canvasGroup.alpha += UnityEngine.Time.unscaledDeltaTime * alphaSpeed;
                    if (canvasGroup.alpha > 1)
                    {
                        canvasGroup.alpha = 1;
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// 获取绑定器
        /// </summary>
        /// <returns></returns>
        public UIComponentBinder GetBinder()
        {
            return binder;
        }

        protected sealed override void OnDestroy()
        {
            Destroy();
            base.OnDestroy();
        }

        /// <summary>
        /// 控制器失活后销毁前执行
        /// </summary>
        public virtual void Destroy()
        {
            
        }
    }
}
