using Framework;
using Game;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreNormalAttackState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        IBattleEntityObject battleEntity = animator.GetComponentInParent<IBattleEntityObject>();

        if (battleEntity is PlayerObject && stateInfo.IsName("PreNormalAttack") || battleEntity is MonsterObject && stateInfo.IsName("IdleBattle"))
        {
            battleEntity.GetComponent<BattleAnimationComponent>().ResetAnimationType();
        }
    }
}
