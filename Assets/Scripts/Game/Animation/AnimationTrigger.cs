using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动画触发器
/// 作为传递动画事件触发的中间层
/// </summary>
[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class AnimationTrigger : MonoBehaviour
{
    public event Action<int> OnAttack;

    /// <summary>
    /// 攻击触发
    /// 动画事件回调
    /// </summary>
    public void OnAttackTrigger(int skillId)
    {
        // 新增：打印帧号+当前动画进度+是否在Attack状态
        AnimatorStateInfo stateInfo = GetComponent<Animator>().GetCurrentAnimatorStateInfo(1);
        LogManager.Log($"{gameObject.name}：触发攻击：ID为{skillId} | 帧号：{Time.frameCount} | NormalizedTime：{stateInfo.normalizedTime:F2} | 是否Attack状态：{stateInfo.IsName("Attack")}");
        OnAttack?.Invoke(skillId);
    }
}
