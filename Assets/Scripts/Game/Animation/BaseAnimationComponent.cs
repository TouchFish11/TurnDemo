using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基础动画组件
/// </summary>
public abstract class BaseAnimationComponent : BaseComponent
{
    // 动画控制器
    protected Animator animator;
    // 动画参数
    protected AnimationParameter animationArg;
    // 动画类型
    protected E_AnimationType currentAnimationType = E_AnimationType.None;

    public override void Init(IEntityObject entityObject)
    {
        animationArg = new AnimationParameter();
        animator = this.EntityObject.GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// 设置动画类型
    /// </summary>
    /// <param name="animationType"></param>
    public void SetAnimationState(E_AnimationType animationType)
    {
        switch (animationType)
        {
            case E_AnimationType.None:
                break;
            case E_AnimationType.Idle:
                animator.SetBool(animationArg.IsRunHash, false);
                break;
            case E_AnimationType.Run:
                animator.SetBool(animationArg.IsRunHash, true);
                break;
            case E_AnimationType.PreNormalAttack:
                animator.SetTrigger(animationArg.PreNormalAttackTriggerHash);
                break;
            case E_AnimationType.NormalAttack:
                animator.SetTrigger(animationArg.NormalAtkTirggerHash);
                break;
            case E_AnimationType.PreBattleAttack:
                animator.SetTrigger(animationArg.PreBattleAttackTriggerHash);
                break;
            case E_AnimationType.BattleAttack:
                animator.SetTrigger(animationArg.BattleAtkTriggerHash);
                break;
            case E_AnimationType.PreUltimateAttack:
                animator.SetTrigger(animationArg.PreUltimateAttackTriggerHash);
                break;
            case E_AnimationType.UltimateAttack:
                animator.SetTrigger(animationArg.UltimateAtkTriggerHash);
                break;
            case E_AnimationType.Hit:
                animator.SetTrigger(animationArg.HitTriggerHash);
                break;
            case E_AnimationType.Death:
                animator.SetTrigger(animationArg.DeathTriggerHash);
                break;
            case E_AnimationType.Rebirth:
                animator.SetTrigger(animationArg.RebirthTriggerHash);
                break;
            default:
                break;
        }
        currentAnimationType = animationType;
    }
}
