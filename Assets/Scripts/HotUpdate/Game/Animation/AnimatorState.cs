using HotUpdate.Base.Animation;
using HotUpdate.Base.Utility;
using UnityEngine;

namespace HotUpdate.Game.Animation
{
    /// <summary>
    /// Animator状态对象
    /// </summary>
    public class AnimatorState
    {
        /// <summary>
        /// 动画配置
        /// </summary>
        public AnimationConfig Config { get; private set; }
        
        public int FullPathHash { get; private set; }
        
        public AnimatorState(AnimationConfig config)
        {
            Config = config;
            var layerName = AnimationLayer.LayerEnumToName(config.layer);
            FullPathHash = Animator.StringToHash($"{layerName}.{config.animationStateName}");
        }
    }
}
