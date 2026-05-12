using HotUpdate.Base.Main;
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
            if (animator.GetComponentInParent<IMainPlayer>() == null || animator.GetComponentInParent<IMainPlayer>().GetComponent<IMoveComponent>() == null)
            {
                return;
            }

            // �����ƶ�
            animator.GetComponentInParent<IMainPlayer>().GetComponent<IMoveComponent>().SetMoveFlag(false);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.GetComponentInParent<IMainPlayer>() == null || animator.GetComponentInParent<IMainPlayer>().GetComponent<IMoveComponent>() == null)
            {
                return;
            }

            // ����ƶ�
            animator.GetComponentInParent<IMainPlayer>().GetComponent<IMoveComponent>().SetMoveFlag(true);
        }
    }
}
