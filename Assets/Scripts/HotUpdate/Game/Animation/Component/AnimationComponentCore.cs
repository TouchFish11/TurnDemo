using HotUpdate.Base.Animation;
using HotUpdate.Base.Component;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Animation.Component
{
    public abstract class AnimationComponentCore<T> : ComponentCore<T> where  T : IAnimationComponent
    {
        /// <summary>
        /// 动画控制器组件
        /// </summary>
        public AnimatorComponent AnimatorComponent {get; private set;}

        /// <summary>
        /// 当前播放的动画状态名称
        /// </summary>
        public abstract string AnimationState { get; protected set; }

        /// <summary>
        /// 动画参数
        /// </summary>
        protected AnimationParameter AnimationParameter { get; private set; }

        protected override void OnInit()
        {
            AnimationParameter = new AnimationParameter();
            AnimatorComponent = Component.EntityObject.GetComponent<AnimatorComponent>();
        }

        /// <summary>
        /// 设置动画类型
        /// </summary>
        /// <param name="type"></param>
        public void SetCommonState(EAnimationType type)
        {
            AnimatorComponent.PlayCommon(type);
            // 更新当前动画类型
            AnimationState = type.ToString();
        }

        /// <summary>
        /// 设置指定动画类型
        /// </summary>
        /// <param name="stateName"></param>
        public void SetState(string stateName)
        {
            AnimatorComponent.Play(stateName);
            // 更新当前动画类型
            AnimationState = stateName;
        }

        /// <summary>
        /// 获取Animator
        /// </summary>
        /// <returns></returns>
        public Animator GetAnimator() => AnimatorComponent.Animator;

        /// <summary>
        /// 获取动画参数
        /// </summary>
        /// <returns></returns>
        public AnimationParameter GetParameter() => AnimationParameter;

        /// <summary>
        /// 获取当前动画状态信息
        /// </summary>
        /// <returns></returns>
        public AnimatorStateInfo GetCurrentAnimatorStateInfo(string layerName)
        {
            if (AnimatorComponent)
            {
                return AnimatorComponent.Animator.GetCurrentAnimatorStateInfo(AnimatorComponent.Animator.GetLayerIndex(layerName));
            }
            
            Logger.LogError($"动画控制器为null");
            return new AnimatorStateInfo();
        }
    }
}
