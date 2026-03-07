using HotUpdate.Animation.Component;
using HotUpdate.Core.Battle.Object;
using UnityEngine;

namespace HotUpdate.Animation.StateMachineBehaviours
{
    /// <summary>
    /// ս������״̬
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
}
