using System;
using System.Collections;
using Core.Singleton;
using GameHotUpdate.Animation.Component;
using GameHotUpdate.Battle.Object;
using UnityEngine;

namespace GameHotUpdate.Animation
{
    /// <summary>
    /// �������Ź�����
    /// ���������Ĵ������¼���Ϊ
    /// </summary>
    public class AnimationPlayManager : SingletonBase<AnimationPlayManager>, IAnimationPlayManager
    {
        private AnimationPlayManager()
        {

        }

        /// <summary>
        /// ���Ŷ���
        /// </summary>
        /// <param name="battleEntity"></param>
        /// <param name="animationType"></param>
        /// <param name="layerName"></param>
        /// <param name="animName"></param>
        /// <param name="overCallBack"></param>
        /// <param name="maxNormalizedTime"></param>
        /// <returns></returns>
        public IEnumerator PlayAnimation(IBattleEntityObject battleEntity, E_AnimationType animationType, string layerName, string animName, Action overCallBack = null, float maxNormalizedTime = 0.9f)
        {
            BattleAnimationComponent animationComponent = battleEntity.GetComponent<BattleAnimationComponent>();
            animationComponent.SetAnimationState(animationType);
            // �ȴ������л�Ϊָ������
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(layerName).IsName(animName));
            // �ȴ��������Ž���
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).normalizedTime >= maxNormalizedTime);
            // ִ�н����ص�
            overCallBack?.Invoke();
        }

        /// <summary>
        /// �ȴ��������Ž���
        /// </summary>
        /// <param name="battleAnimationComponent"></param>
        /// <param name="layerName"></param>
        /// <param name="animationType"></param>
        /// <param name="playOver"></param>
        /// <returns></returns>
        public IEnumerator WaitForAnimOver(BattleAnimationComponent battleAnimationComponent, string layerName, E_AnimationType animationType, Action playOver = null)
        {
            battleAnimationComponent.SetAnimationState(animationType);
            // �ȴ������л�Ϊָ������
            yield return new WaitUntil(() => battleAnimationComponent.GetCurrentAnimatorStateInfo(layerName).IsName(animationType.ToString()));
            // �ȴ������������
            yield return new WaitUntil(() => battleAnimationComponent.GetCurrentAnimatorStateInfo(layerName).normalizedTime >= 0.9);
            playOver?.Invoke();
        }
    }
}
