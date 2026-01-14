using Framework;
using Game.Battle;
using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 动画播放管理器
    /// 管理动画的触发、事件行为
    /// </summary>
    public class AnimationPlayManager : SingletonBase<AnimationPlayManager>
    {
        private AnimationPlayManager()
        {

        }

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="battleEntity"></param>
        /// <param name="animationType"></param>
        /// <param name="animName"></param>
        /// <param name="callback"></param>
        /// <param name="normalTimes"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 动画播放到结束
        /// </summary>
        /// <param name="battleAnimationComponent"></param>
        /// <param name="animationType"></param>
        /// <param name="callBack"></param>
        public void PlayAnimationOver(BattleAnimationComponent battleAnimationComponent, E_AnimationType animationType, Action callBack = null)
        {
            battleAnimationComponent.StartCoroutine(WaitForAnimOver(battleAnimationComponent, animationType, callBack));
        }

        /// <summary>
        /// 等待动画播放结束
        /// </summary>
        /// <param name="battleAnimationComponent"></param>
        /// <param name="animationType"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public IEnumerator WaitForAnimOver(BattleAnimationComponent battleAnimationComponent, E_AnimationType animationType, Action callBack)
        {
            battleAnimationComponent.SetAnimationState(animationType);
            // 等待动画切换为指定动画
            yield return new WaitUntil(() => battleAnimationComponent.GetCurrentAnimatorStateInfo().IsName(animationType.ToString()));

            // 等待动画播放完毕
            yield return new WaitUntil(() => battleAnimationComponent.GetCurrentAnimatorStateInfo().normalizedTime >= 0.9);

            callBack?.Invoke();
        }
    }
}
