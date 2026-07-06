using HotUpdate.Base.Animation;

namespace HotUpdate.Game.Animation.Component
{
    /// <summary>
    /// 战斗动画组件
    /// </summary>
    public class BattleAnimationComponentCore : AnimationComponentCore<BattleAnimationComponent>
    {
        public override string AnimationState { get; protected set; }

        // public override void SetCommonState(int type)
        // {
        //     // var animationType = (E_AnimationType)type;
        //     // // 临时逻辑：若当前已在播放预普通攻击动画，且目标动画也是预普通攻击，则不重复触发
        //     // if (AnimatorComponent.Animator.GetCurrentAnimatorStateInfo(AnimatorComponent.Animator.GetLayerIndex(AnimationUtility.Battle_Layer_Name)).IsName("PreNormalAttack") 
        //     //     && animationType == E_AnimationType.PreNormalAttack)
        //     // {
        //     //     return;
        //     // }
        //     
        //     // 根据动画类型触发对应的Animator Trigger
        //     // switch (animationType)
        //     // {
        //     //     case E_AnimationType.None: // 无动画
        //     //         break;
        //     //     case E_AnimationType.PreNormalAttack: // 预普通攻击
        //     //         AnimatorComponent.Animator.SetTrigger(AnimationParameter.PreNormalAttackTriggerHash);
        //     //         break;
        //     //     case E_AnimationType.NormalAttack: // 普通攻击
        //     //         AnimatorComponent.Animator.SetTrigger(AnimationParameter.NormalAtkTirggerHash);
        //     //         break;
        //     //     case E_AnimationType.PreBattleAttack: // 预战斗技能攻击
        //     //         AnimatorComponent.Animator.SetTrigger(AnimationParameter.PreBattleAttackTriggerHash);
        //     //         break;
        //     //     case E_AnimationType.BattleAttack: // 战斗技能攻击
        //     //         AnimatorComponent.Animator.SetTrigger(AnimationParameter.BattleAtkTriggerHash);
        //     //         break;
        //     //     case E_AnimationType.PreUltimateAttack: // 预必杀技攻击
        //     //         AnimatorComponent.Animator.SetTrigger(AnimationParameter.PreUltimateAttackTriggerHash);
        //     //         break;
        //     //     case E_AnimationType.UltimateAttack: // 必杀技攻击
        //     //         AnimatorComponent.Animator.SetTrigger(AnimationParameter.UltimateAtkTriggerHash);
        //     //         break;
        //     //     case E_AnimationType.Hit: // 受击
        //     //         AnimatorComponent.Animator.SetTrigger(AnimationParameter.HitTriggerHash);
        //     //         break;
        //     //     case E_AnimationType.Death: // 死亡
        //     //         AnimatorComponent.Animator.SetTrigger(AnimationParameter.DeathTriggerHash);
        //     //         break;
        //     //     case E_AnimationType.Rebirth: // 复活
        //     //         AnimatorComponent.Animator.SetTrigger(AnimationParameter.RebirthTriggerHash);
        //     //         break;
        //     //     case E_AnimationType.Attack: // 通用攻击（怪物默认）
        //     //         AnimatorComponent.Animator.SetTrigger(AnimationParameter.AttackTirggerHash);
        //     //         break;
        //     // }
        // }
    }
}
