using System.Threading.Tasks;
using Core.Time;
using UnityEngine;

namespace Core.UI.ViewController
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIView : UIBehaviourBase, IuiView
    {
        // 画布组
        protected CanvasGroup canvasGroup;
        
        /// <summary>
        ///  淡入速度（过渡秒数）
        /// <value>默认：0.05f</value>
        /// </summary>
        protected float FadeInSpeed { get; set; } = 0.05f; 
        
        /// <summary>
        ///  淡出速度（过渡秒数）
        /// <value>默认：0.15f</value>
        /// </summary>
        protected float FadeOutSpeed { get; set; }  = 0.15f;
        
        public GameObject ViewObj { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            canvasGroup = GetComponent<CanvasGroup>();
            ViewObj = gameObject;
            canvasGroup.alpha = 0;
        }

        /// <summary>
        /// 淡入
        /// </summary>
        public async Task FadeIn()
        {
            while (true)
            {
                canvasGroup.alpha += TimeUtil.UnscaledDeltaTime * (1 / FadeInSpeed);
                if (canvasGroup.alpha >= 1)
                {
                    canvasGroup.alpha = 1;
                    return;
                }
                await Task.Yield();
            }
        }

        /// <summary>
        /// 淡出
        /// </summary>
        public async Task FadeOut()
        {
            while (true)
            {
                canvasGroup.alpha -= TimeUtil.UnscaledDeltaTime * (1 / FadeOutSpeed);
                if (canvasGroup.alpha <= 0)
                {
                    canvasGroup.alpha = 0;
                    return;
                }
                await Task.Yield();
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

        /// <summary>
        /// 控制器销毁后执行，用于自身的清理逻辑，不依赖控制器
        /// </summary>
        public virtual void Destroy()
        {

        }
        
        protected sealed override void OnDestroy()
        {

        }
    }
}
