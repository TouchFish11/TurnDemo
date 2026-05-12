using HotUpdate.Base.Battle.Object;
using HotUpdate.Game.Animation.Component;
using UnityEngine;

namespace HotUpdate.Game.Animation.StateMachineBehaviours
{
    /// <summary>
    /// ս������״̬
    /// </summary>
    public class IdleBattleState : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            IBattleEntityObject battleEntity = animator.GetComponentInParent<IBattleEntityObject>();
            if (battleEntity is IMonsterObject && stateInfo.IsName("IdleBattle"))
            {
                battleEntity.GetComponent<BattleAnimationComponent>().ResetAnimationType();
            }
        }
    }
}
