using Framework;
using Game.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;

/// <summary>
/// 动画播放管理器
/// 管理动画的触发、事件行为
/// </summary>
public class AnimationPlayManager : SingletonBase<AnimationPlayManager>
{
    // 通过反射创建实例，由一个全局管理器来管理实例，避免过多的单例对象

    private AnimationPlayManager()
    {

    }

    public IEnumerator PlayAnimation(IBattleEntityObject battleEntity, E_AnimationType animationType, string animName, Action<float> callback, params float[] normalTimes)
    {
        BattleAnimationComponent animationComponent = battleEntity.GetComponent<BattleAnimationComponent>();
        animationComponent.SetAnimationState(animationType);
        // 等待动画切换为攻击动画
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo().IsName(animName));

        int index = 0;
        while (index < normalTimes.Length)
        {
            float currentTime = normalTimes[index];
            if (animationComponent.GetCurrentAnimatorStateInfo().normalizedTime >= currentTime)
            {
                callback?.Invoke(currentTime);
                index++;
            }

            yield return null;
        }
    }

}
