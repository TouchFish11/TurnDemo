using System;
using System.Collections.Generic;
using Core.SO;
using HotUpdate.Base.Utility;
using UnityEngine;

namespace HotUpdate.Base.Animation
{
    /// <summary>
    /// 动画配置
    /// </summary>
    [CreateAssetMenu(fileName = "AnimationConfigCollection", menuName = "Animation/AnimationConfigCollection")]
    public class AnimationConfigCollectionSO : SOBase
    {
        [SerializeField] private AnimationConfigCollectionSO commonCollectionSO;
        
        [SerializeField] private AnimationConfigCollection animationConfigCollection;
        
        private void OnValidate()
        {
            foreach (var animationConfig in animationConfigCollection.animationConfigs)
            {
                // 根据层级和状态名称自动计算hash
                var layerName = AnimationLayer.LayerEnumToName(animationConfig.layer);
                var nameWithLayer = $"{layerName}.{animationConfig.animationStateName}";
                animationConfig.animationHash = Animator.StringToHash(nameWithLayer);
            }

            animationConfigCollection.commonCollection = commonCollectionSO?.animationConfigCollection;
            target = animationConfigCollection;
        }

        protected override void OnAwake()
        {
            if (animationConfigCollection == null)
            {
                animationConfigCollection = new AnimationConfigCollection
                {
                    commonCollection = commonCollectionSO?.animationConfigCollection,
                    animationConfigs = new List<AnimationConfig>()
                };
            }
        }
    }
}