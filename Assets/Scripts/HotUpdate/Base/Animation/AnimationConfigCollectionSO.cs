using System.Collections.Generic;
using System.Text;
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
            var sb = new StringBuilder();
            foreach (var animationConfig in animationConfigCollection.animationConfigs)
            {
                // 根据层级和状态名称自动计算hash
                sb.Clear();
                var layerName = AnimationLayer.LayerEnumToName(animationConfig.layer);
                sb.Append(layerName);
                foreach (var stateMachineName in animationConfig.subStateMachineNames)
                {
                    sb.Append(".");
                    sb.Append(stateMachineName);
                }
                
                sb.Append(".");
                sb.Append(animationConfig.animationStateName);
                animationConfig.animationHash = Animator.StringToHash(sb.ToString());
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