using System;
using System.Collections;
using GameHotUpdate.Animation.Component;
using GameHotUpdate.Battle.Object;

namespace GameHotUpdate.Animation
{
    public interface IAnimationPlayManager
    {
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
        IEnumerator PlayAnimation(IBattleEntityObject battleEntity, E_AnimationType animationType, string layerName, string animName, Action overCallBack = null, float maxNormalizedTime = 0.9f);
        
        /// <summary>
        /// �ȴ��������Ž���
        /// </summary>
        /// <param name="battleAnimationComponent"></param>
        /// <param name="layerName"></param>
        /// <param name="animationType"></param>
        /// <param name="playOver"></param>
        /// <returns></returns>
        IEnumerator WaitForAnimOver(BattleAnimationComponent battleAnimationComponent, string layerName, E_AnimationType animationType, Action playOver = null);
    }
}
