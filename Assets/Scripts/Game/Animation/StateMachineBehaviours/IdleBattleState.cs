using Framework;
using Game;
using Game.Battle;
using UnityEngine;

/// <summary>
/// Õ½¶·´ý»ú×´Ì¬
/// </summary>
public class IdleBattleState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        IBattleEntityObject battleEntity = animator.GetComponentInParent<IBattleEntityObject>();
        if (battleEntity is MonsterObject && stateInfo.IsName("IdleBattle"))
        {
            battleEntity.GetComponent<BattleAnimationComponent>().ResetAnimationType();
        }
    }
}
