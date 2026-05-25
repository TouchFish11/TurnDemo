using HotUpdate.Base;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Object;
using UnityEngine;

namespace HotUpdate.Game.Animation.StateMachineBehaviours
{
    /// <summary>
    /// Ԥ�չ�״̬
    /// </summary>
    public class PreNormalAttackState : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            IBattleEntityObject battleEntity = animator.GetComponentInParent<IBattleEntityObject>();
            if (battleEntity is IPlayerObject && stateInfo.IsName("PreNormalAttack"))
            {
                var animationComponent = battleEntity.GetComponent<BattleAnimationComponent>();
                animationComponent.ResetAnimationType();
                //animator.ResetTrigger(animationComponent.GetParameter().PreNormalAttackTriggerHash);
            }
        }
    }
}
