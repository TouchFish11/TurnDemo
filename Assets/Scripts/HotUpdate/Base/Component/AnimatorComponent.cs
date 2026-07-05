using HotUpdate.Base.Animation;
using UnityEngine;

namespace HotUpdate.Base.Component
{
    /// <summary>
    /// 动画控制器组件
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [ComponentCore(typeof(AnimatorComponentCore))]
    public class AnimatorComponent : BaseComponent
    {
        private AnimatorComponentCore _animatorComponentCore;
        
        /// <summary>
        /// Unity动画状态机
        /// </summary>
        public Animator Animator { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Animator = GetComponent<Animator>();
        }

        protected override void OnInit()
        {
            _animatorComponentCore = (AnimatorComponentCore)ComponentCore;
        }

        /// <summary>
        /// 播放指定类型动画
        /// </summary>
        /// <param name="animationType"></param>
        public void Play(EAnimationType animationType)
        {
            _animatorComponentCore.Play(animationType);
        }

        /// <summary>
        /// 设置层级全重
        /// </summary>
        /// <param name="layerIndex"></param>
        /// <param name="weight"></param>
        public void SetLayerWeight(EAnimationLayer layerIndex, float weight)
        {
            Animator.SetLayerWeight((int)layerIndex, weight);
        }

        protected override void OnBaseDestroy()
        {
            _animatorComponentCore = null;
            Animator = null;
        }
    }
}
