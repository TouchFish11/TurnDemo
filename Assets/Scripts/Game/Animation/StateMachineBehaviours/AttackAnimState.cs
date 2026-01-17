using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ¹¥»÷¶¯»­×´Ì¬
/// </summary>
public class AttackAnimState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponentInParent<MainPlayer>() == null || animator.GetComponentInParent<MainPlayer>().GetComponent<MoveComponent>() == null)
        {
            return;
        }

        // ½ûÓÃÒÆ¶¯
        animator.GetComponentInParent<MainPlayer>().GetComponent<MoveComponent>().SetMoveFlag(false);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponentInParent<MainPlayer>() == null || animator.GetComponentInParent<MainPlayer>().GetComponent<MoveComponent>() == null)
        {
            return;
        }

        // ½â½ûÒÆ¶¯
        animator.GetComponentInParent<MainPlayer>().GetComponent<MoveComponent>().SetMoveFlag(true);
    }
}
