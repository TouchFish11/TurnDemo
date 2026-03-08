using System;
using System.Collections;
using System.Threading.Tasks;
using Core.Singleton;
using HotUpdate.Core.Animation;
using HotUpdate.Core.Battle.Object;
using UnityEngine;

namespace HotUpdate.Battle
{
    public class AnimationPlayManager : IInitializable, IAnimationPlayManager
    {
        public int Priority => -1;

        public Task InitAsync()
        {
            return Task.CompletedTask;
        }
        
        public IEnumerator PlayAnimation(IBattleEntityObject battleEntity, int type, string layerName, string animName,
            Action overCallBack = null, float maxNormalizedTime = 0.9F)
        {
            var animationComponent = battleEntity.GetComponent<IBattleAnimationComponent>();
            animationComponent.SetAnimationState(type);

            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(layerName).IsName(animName));

            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).normalizedTime >= maxNormalizedTime);

            overCallBack?.Invoke();
        }

        public IEnumerator WaitForAnimOver(IBattleAnimationComponent battleAnimationComponent, string layerName,
            int type, Action playOver = null)
        {
            battleAnimationComponent.SetAnimationState(type);

            yield return new WaitUntil(() => battleAnimationComponent.GetCurrentAnimatorStateInfo(layerName).IsName(((E_AnimationType)type).ToString()));

            yield return new WaitUntil(() => battleAnimationComponent.GetCurrentAnimatorStateInfo(layerName).normalizedTime >= 0.9);
            playOver?.Invoke();
        }
    }
}
