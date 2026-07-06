using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotUpdate.Base.Animation
{
    /// <summary>
    /// 动画配置集合
    /// </summary>
    [Serializable]
    public class AnimationConfigCollection
    {
        /// 实体通用动画集合
        [SerializeField] public AnimationConfigCollection commonCollection;
        /// 实体差异动画集合；也是顶层通用集合的配置集合
        [SerializeField] public List<AnimationConfig> animationConfigs;
    }
}
