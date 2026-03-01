using GameHotUpdate.Animation.Component;
using GameHotUpdate.Battle.Object;
using UnityEngine;

namespace GameHotUpdate.Animation.StateMachineBehaviours
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
