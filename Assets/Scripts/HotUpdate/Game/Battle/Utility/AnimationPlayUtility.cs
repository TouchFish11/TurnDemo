using System;
using System.Collections;
using HotUpdate.Base;
using HotUpdate.Base.Component;
using HotUpdate.Base.Enums;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Object;
using UnityEngine;

namespace HotUpdate.Game.Battle.Utility
{
    /// <summary>
    /// 动画播放工具类
    /// </summary>
    public static class AnimationPlayUtility
    {
        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="battleEntity"></param>
        /// <param name="type"></param>
        /// <param name="layerName"></param>
        /// <param name="animName"></param>
        /// <param name="overCallBack"></param>
        /// <param name="maxNormalizedTime"></param>
        /// <returns></returns>
        public static IEnumerator PlayAnimation(IBattleEntityObject battleEntity, int type, string layerName, string animName, Action overCallBack = null, float maxNormalizedTime = 0.9F)
        {
            var animationComponent = battleEntity.GetComponent<IBattleAnimationComponent>();
            animationComponent.SetAnimationState(type);
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(layerName).IsName(animName));
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).normalizedTime >= maxNormalizedTime);
            overCallBack?.Invoke();
        }

        /// <summary>
        /// 等待动画播放完成
        /// </summary>
        /// <param name="battleAnimationComponent"></param>
        /// <param name="layerName"></param>
        /// <param name="type"></param>
        /// <param name="playOver"></param>
        /// <returns></returns>
        public static IEnumerator WaitForAnimOver(IBattleAnimationComponent battleAnimationComponent, string layerName, int type, Action playOver = null)
        {
            battleAnimationComponent.SetAnimationState(type);
            yield return new WaitUntil(() => battleAnimationComponent.GetCurrentAnimatorStateInfo(layerName).IsName(((E_AnimationType)type).ToString()));
            yield return new WaitUntil(() => battleAnimationComponent.GetCurrentAnimatorStateInfo(layerName).normalizedTime >= 0.9);
            playOver?.Invoke();
        }
    }
}
