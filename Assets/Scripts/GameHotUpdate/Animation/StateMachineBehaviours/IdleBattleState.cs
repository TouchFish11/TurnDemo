using GameHotUpdate.Animation.Component;
using GameHotUpdate.Battle.Object;
using UnityEngine;

namespace GameHotUpdate.Animation.StateMachineBehaviours
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
