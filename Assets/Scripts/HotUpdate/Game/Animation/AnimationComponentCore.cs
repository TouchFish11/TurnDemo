using HotUpdate.Base.Animation;
using HotUpdate.Base.Component;
using HotUpdate.Base.Enums;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Animation
{
    public abstract class AnimationComponentCore<T> : ComponentCore<T> where  T : IAnimationComponent
    {
        /// <summary>
        /// 动画控制器组件
        /// </summary>
        public AnimatorComponent AnimatorComponent {get; private set;}

        /// <summary>
        /// 当前动画类型
        /// </summary>
        public abstract E_AnimationType CurrentAnimationType { get; set; }

        /// <summary>
        /// 动画参数
        /// </summary>
        protected AnimationParameter AnimationParameter { get; private set; }

        protected override void OnInit()
        {
            AnimationParameter = new AnimationParameter();
            AnimatorComponent = Component.EntityObject.GetComponentInChildren<AnimatorComponent>();
        }

        /// <summary>
        /// 设置动画类型
        /// </summary>
        /// <param name="type"></param>
        public abstract void SetAnimationState(int type);

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
