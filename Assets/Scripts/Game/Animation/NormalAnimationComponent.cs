using UnityEngine;

namespace Game
{
    /// <summary>
    /// 非战斗动画组件
    /// </summary>
    public class NormalAnimationComponent : AnimationComponent
    {
        public override int LayerIndex { get; protected set; }

        public override void Init(IEntityObject entityObject)
        {
            base.Init(entityObject);
            LayerIndex = animator.GetLayerIndex("Base Layer");
            this.EntityObject.GetComponent<InputComponent>().OnKeyInputChanged += OnMove;
            this.EntityObject.GetComponent<InputComponent>().OnMouseLeftClick += OnAttack;
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
