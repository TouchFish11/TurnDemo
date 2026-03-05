using HotUpdate.Main;
using HotUpdate.Main.Move;
using UnityEngine;

namespace HotUpdate.Animation.StateMachineBehaviours
{
    /// <summary>
    /// ��������״̬
    /// </summary>
    public class AttackAnimState : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.GetComponentInParent<MainPlayer>() == null || animator.GetComponentInParent<MainPlayer>().GetComponent<MoveComponent>() == null)
            {
                return;
            }

            // �����ƶ�
            animator.GetComponentInParent<MainPlayer>().GetComponent<MoveComponent>().SetMoveFlag(false);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.GetComponentInParent<MainPlayer>() == null || animator.GetComponentInParent<MainPlayer>().GetComponent<MoveComponent>() == null)
            {
                return;
            }

            // ����ƶ�
            animator.GetComponentInParent<MainPlayer>().GetComponent<MoveComponent>().SetMoveFlag(true);
        }
    }
}
