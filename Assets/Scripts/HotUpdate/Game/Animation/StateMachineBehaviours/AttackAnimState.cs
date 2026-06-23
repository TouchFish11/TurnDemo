using HotUpdate.Game.Main.Move;
using HotUpdate.Game.Main.Player;
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
            if (!animator.GetComponentInParent<MainPlayer>() || animator.GetComponentInParent<MainPlayer>().GetComponent<MoveComponent>() == null)
            {
                return;
            }
            
            animator.GetComponentInParent<MainPlayer>().GetComponent<MoveComponent>().SetMoveFlag(false);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!animator.GetComponentInParent<MainPlayer>() || animator.GetComponentInParent<MainPlayer>().GetComponent<MoveComponent>() == null)
            {
                return;
            }
            
            animator.GetComponentInParent<MainPlayer>().GetComponent<MoveComponent>().SetMoveFlag(true);
        }
    }
}
