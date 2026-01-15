using UnityEngine;

namespace Game
{
    /// <summary>
    /// 非战斗动画组件
    /// </summary>
    public class NormalAnimationComponent : AnimationComponent
    {
        protected override E_AnimationType CurrentAnimationType { get; set; } = E_AnimationType.None;

        public override void Init(IEntityObject entityObject)
        {
            base.Init(entityObject);
            this.EntityObject.GetComponent<InputComponent>().OnKeyInputChanged += OnMove;
            this.EntityObject.GetComponent<InputComponent>().OnMouseLeftClick += OnAttack;
        }

        public override void SetAnimationState(E_AnimationType animationType)
        {
            if (CurrentAnimationType == animationType)
            {
                return;
            }

            switch (animationType)
            {
                case E_AnimationType.None:

                    break;
                case E_AnimationType.Idle:

                    break;
                case E_AnimationType.Run:

                    break;
                case E_AnimationType.NormalAttack:

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
