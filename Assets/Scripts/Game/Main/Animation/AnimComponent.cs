
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
        // 移动参数hash
        private readonly int isRunHash = Animator.StringToHash("isRun");
        // 攻击触发参数hash
        private readonly int AttackTriggerHash = Animator.StringToHash("AttackTrigger");

        protected override void Awake()
        {
            base.Awake();
            animator = this.EntityObject.GetComponentInChildren<Animator>();
            this.EntityObject.GetComponent<InputComponent>().OnKeyInputChanged += OnMove;
            this.EntityObject.GetComponent<InputComponent>().OnMouseLeftClick += OnAttack;
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="inputDir"></param>
        private void OnMove(Vector3 inputDir)
        {
            animator.SetBool(isRunHash, inputDir != Vector3.zero);
        }

        /// <summary>
        /// 攻击
        /// </summary>
        private void OnAttack()
        {
            animator.SetTrigger(AttackTriggerHash);
        }
    }
}
