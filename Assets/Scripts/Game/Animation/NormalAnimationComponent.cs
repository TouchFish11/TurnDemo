using UnityEngine;

namespace Game
{
    /// <summary>
    /// 非战斗动画组件
    /// </summary>
    [ComponentId(nameof(NormalAnimationComponent))]
    public class NormalAnimationComponent : AnimationComponent
    {
        protected override E_AnimationType CurrentAnimationType { get; set; } = E_AnimationType.None;

        public override void Init(IEntityObject entityObject)
        {
            base.Init(entityObject);
            this.EntityObject.GetComponent<InputComponent>().OnKeyInputChanged += OnMove;
            this.EntityObject.GetComponent<InputComponent>().OnMouseLeftClick += OnAttack;
        }

        /// <summary>
        /// 设置状态机
        /// </summary>
        /// <param name="animator"></param>
        public void SetAnimator(Animator animator)
        {
            this.animator = animator;

            // 设置战斗相关层级权重为0
            animator.SetLayerWeight(animator.GetLayerIndex(Battle_Layer_Name), 0);
            animator.SetLayerWeight(animator.GetLayerIndex(Skill_Layer_Name), 0);
        }

        public override void SetAnimationState(E_AnimationType animationType)
        {
            switch (animationType)
            {
                case E_AnimationType.Idle:
                    animator.SetBool(animationArg.IsRunHash, false);
                    break;
                case E_AnimationType.Run:
                    animator.SetBool(animationArg.IsRunHash, true);
                    break;
                case E_AnimationType.NormalAttack:
                    animator.SetTrigger(animationArg.NormalAtkTirggerHash);
                    break;
            }
            CurrentAnimationType = animationType;
        }

        /// <summary>
        /// 移动事件回调
        /// </summary>
        /// <param name="inputDir"></param>
        private void OnMove(Vector3 inputDir)
        {
            SetAnimationState(inputDir != Vector3.zero ? E_AnimationType.Run : E_AnimationType.Idle);
        }

        /// <summary>
        /// 攻击事件回调
        /// </summary>
        private void OnAttack()
        {
            SetAnimationState(E_AnimationType.NormalAttack);
        }


    }
}
