using System.Threading.Tasks;
using Core.Tasks;
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
        
        public GameObject ViewObj { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            canvasGroup = GetComponent<CanvasGroup>();
            ViewObj = gameObject;
            _isHide = false;
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
                        _isHide = true;
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
                        _isHide = false;
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

        }

        /// <summary>
        /// 控制器销毁后执行，用于自身的清理逻辑，不依赖控制器
        /// </summary>
        public virtual Task Destroy()
        {
            return TaskUtility.WaitUntil(() => _isHide);
        }
    }
}
