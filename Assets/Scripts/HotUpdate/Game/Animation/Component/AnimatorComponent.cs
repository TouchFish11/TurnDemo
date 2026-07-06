using HotUpdate.Base.Animation;
using HotUpdate.Base.Component;
using UnityEngine;

namespace HotUpdate.Game.Animation.Component
{
    /// <summary>
    /// 动画控制器组件
    /// </summary>
    [ComponentCore(typeof(AnimatorComponentCore))]
    [ComponentId(typeof(AnimatorComponent))]
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
            Animator = GetComponentInChildren<Animator>();
        }

        protected override void OnInit()
        {
            _animatorComponentCore = (AnimatorComponentCore)ComponentCore;
        }

        /// <summary>
        /// 播放指定类型通用动画
        /// </summary>
        /// <param name="animationType"></param>
        public void PlayCommon(EAnimationType animationType)
        {
            _animatorComponentCore.PlayCommon(animationType);
        }

        /// <summary>
        /// 播放指定类型动画
        /// </summary>
        /// <param name="stateName"></param>
        public void Play(string stateName)
        {
            _animatorComponentCore.Play(stateName);
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
