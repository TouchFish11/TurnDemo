using HotUpdate.Animation.Component;
using HotUpdate.Core.Battle.Object;
using UnityEngine;

namespace HotUpdate.Animation.StateMachineBehaviours
{
    /// <summary>
    /// Ԥ�չ�״̬
    /// </summary>
    public class PreNormalAttackState : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            IBattleEntityObject battleEntity = animator.GetComponentInParent<IBattleEntityObject>();
            if (battleEntity is PlayerObject && stateInfo.IsName("PreNormalAttack"))
            {
                var animationComponent = battleEntity.GetComponent<BattleAnimationComponent>();
                animationComponent.ResetAnimationType();
                //animator.ResetTrigger(animationComponent.GetParameter().PreNormalAttackTriggerHash);
            }
        }
    }
}
