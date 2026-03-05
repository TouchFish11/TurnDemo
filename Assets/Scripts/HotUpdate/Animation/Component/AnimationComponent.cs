using Core.Components;
using Core.Log;
using HotUpdate.Component;
using UnityEngine;

namespace HotUpdate.Animation.Component
{
    /// <summary>
    /// 基础动画组件
    /// </summary>
    public abstract class AnimationComponent : BaseComponent
    {
        // 动画控制器组件
        protected AnimatorComponent animatorComponent;
        // 动画参数
        protected AnimationParameter animationArg;
        // 动画类型
        protected abstract E_AnimationType CurrentAnimationType { get; set; }

        // 定义动画层级名称常量,与Animator窗口中的层级名称一致
        public const string Base_Layer_Name = "Base Layer";
        public const string Battle_Layer_Name = "Battle Layer";
        public const string Skill_Layer_Name = "Skill Layer";

        /// <summary>
        /// 动画参数
        /// </summary>
        public AnimationParameter AnimationParameter => animationArg;

        public override void Init(IEntityObject entityObject)
        {
            animationArg = new AnimationParameter();
            animatorComponent = entityObject.GetComponentInChildren<AnimatorComponent>();
        }

        /// <summary>
        /// 设置动画类型
        /// </summary>
        /// <param name="animationType"></param>
        public abstract void SetAnimationState(E_AnimationType animationType);

        /// <summary>
        /// 获取Animator
        /// </summary>
        /// <returns></returns>
        public Animator GetAnimator() => animatorComponent.Animator;

        /// <summary>
        /// 获取动画参数
        /// </summary>
        /// <returns></returns>
        public AnimationParameter GetParameter() => animationArg;

        /// <summary>
        /// 获取当前动画状态信息
        /// </summary>
        /// <returns></returns>
        public AnimatorStateInfo GetCurrentAnimatorStateInfo(string layerName)
        {
            if (animatorComponent != null)
            {
                return animatorComponent.Animator.GetCurrentAnimatorStateInfo(animatorComponent.Animator.GetLayerIndex(layerName));
            }
            
            LogManager.LogError($"动画控制器为null");
            return new AnimatorStateInfo();
        }
    }
}
