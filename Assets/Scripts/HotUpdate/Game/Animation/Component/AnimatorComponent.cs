using System;
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
        
        protected override void OnInit()
        {
            Animator = GetComponentInChildren<Animator>();
            _animatorComponentCore = (AnimatorComponentCore)ComponentCore;
            _animatorComponentCore.InitConfigs();
        }

        /// <summary>
        /// 播放指定类型通用动画
        /// </summary>
        /// <param name="animationType"></param>
        public bool PlayCommon(EAnimationType animationType)
        {
            return _animatorComponentCore.PlayCommon(animationType);
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

        /// <summary>
        /// 添加非循环动画结束后事件监听，会在切换为默认动画状态后执行，可用于打断动画结束后恢复到默认状态
        /// 可在其中指定进入的状态
        /// </summary>
        /// <param name="OnAnimationFinished"></param>
        public void AddAnimationFinished(Action<AnimationConfig> OnAnimationFinished)
        {
            _animatorComponentCore.OnAnimationFinished += OnAnimationFinished;
        }

        protected override void OnBaseDestroy()
        {
            _animatorComponentCore = null;
            Animator = null;
        }
    }
}
