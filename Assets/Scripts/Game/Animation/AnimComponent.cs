
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 动画组件
    /// </summary>
    public class AnimComponent : BaseComponent
    {
        // 动画控制器
        private Animator animator;
        // 动画参数
        private AnimationArg animationArg;
        // 动画类型
        private AnimationType currentAnimationType = AnimationType.None;

        protected override void Awake()
        {
            base.Awake();

            animationArg = new AnimationArg();
            animator = this.EntityObject.GetComponentInChildren<Animator>();

            this.EntityObject.GetComponent<InputComponent>().OnKeyInputChanged += OnMove;
            this.EntityObject.GetComponent<InputComponent>().OnMouseLeftClick += OnAttack;
        }

        /// <summary>
        /// 设置动画类型
        /// </summary>
        /// <param name="animationType"></param>
        public void SetAnimationState(AnimationType animationType)
        {
            if (animationType == currentAnimationType)
            {
                return;
            }

            switch (animationType)
            {
                case AnimationType.Idle:
                    animator.SetBool(animationArg.IsRunHash, false);
                    break;
                case AnimationType.Run:
                    animator.SetBool(animationArg.IsRunHash, true);
                    break;
                case AnimationType.Attack:
                    animator.SetTrigger(animationArg.AttackTriggerHash);
                    break;
            }

            currentAnimationType = animationType;
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="inputDir"></param>
        private void OnMove(Vector3 inputDir)
        {
            SetAnimationState(inputDir != Vector3.zero ? AnimationType.Run : AnimationType.Idle);
        }

        /// <summary>
        /// 攻击
        /// </summary>
        private void OnAttack()
        {
            SetAnimationState(AnimationType.Attack);
        }
    }
}
