using HotUpdate.Base.Object;
using HotUpdate.Game.Main.Move;
using UnityEngine;

namespace HotUpdate.Game.Animation.StateMachineBehaviours
{
    /// <summary>
    /// 攻击动画状态
    /// </summary>
    public class AttackAnimState : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var entityObject = animator.GetComponentInParent<EntityObject>();
            var moveComponent = entityObject.GetComponent<MoveComponent>();
            
            if (!entityObject || !moveComponent)
                return;
            
            moveComponent.SetMoveFlag(false);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var entityObject = animator.GetComponentInParent<EntityObject>();
            var moveComponent = entityObject.GetComponent<MoveComponent>();
            if (!entityObject || !moveComponent)
                return;
            
            moveComponent.SetMoveFlag(true);
        }
    }
}
