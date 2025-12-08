using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动画参数
/// </summary>
public class AnimationArg
{
    /// <summary>
    /// 移动参数hash
    /// </summary>
    public int IsRunHash { get; } = Animator.StringToHash("isRun");
    /// <summary>
    /// 攻击触发参数hash
    /// </summary>
    public int AttackTriggerHash { get; } = Animator.StringToHash("AttackTrigger");
}
