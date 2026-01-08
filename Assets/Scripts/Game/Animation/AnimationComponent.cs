using Framework;
using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基础动画组件
/// </summary>
public abstract class AnimationComponent : BaseComponent
{
    // 动画控制器
    protected Animator animator;
    // 动画参数
    protected AnimationParameter animationArg;
    // 动画类型
    protected abstract E_AnimationType CurrentAnimationType { get; set; }

    /// <summary>
    /// 动画层级索引
    /// </summary>
    public abstract int LayerIndex { get; protected set; }

    /// <summary>
    /// 动画参数
    /// </summary>
    public AnimationParameter AnimationParameter => animationArg;

    public override void Init(IEntityObject entityObject)
    {
        animationArg = new AnimationParameter();
        animator = this.EntityObject.GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// 设置动画类型
    /// </summary>
    /// <param name="animationType"></param>
    public abstract void SetAnimationState(E_AnimationType animationType);

    /// <summary>
    /// 获取Animator
    /// </summary>
    /// <returns></returns>
    public Animator GetAnimator() => animator;

    /// <summary>
    /// 获取动画参数
    /// </summary>
    /// <returns></returns>
    public AnimationParameter GetParameter() => animationArg;

    /// <summary>
    /// 获取当前动画状态信息
    /// </summary>
    /// <returns></returns>
    public AnimatorStateInfo GetCurrentAnimatorStateInfo()
    {
        return animator.GetCurrentAnimatorStateInfo(LayerIndex);
    }

}
