using System;
using System.Collections;
using HotUpdate.Core.Battle.Object;

namespace HotUpdate.Core.Animation
{
    public interface IAnimationPlayManager
    {
        IEnumerator PlayAnimation(IBattleEntityObject battleEntity, int type, string layerName, string animName, Action overCallBack = null, float maxNormalizedTime = 0.9f);
        
        IEnumerator WaitForAnimOver(IBattleAnimationComponent battleAnimationComponent, string layerName, int type, Action playOver = null);
    }
}
