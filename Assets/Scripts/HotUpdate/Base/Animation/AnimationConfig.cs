using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace HotUpdate.Base.Animation
{
    /// <summary>
    /// 状态动画配置
    /// </summary>
    [Serializable]
    public class AnimationConfig
    {
        /// 动画层级
        public EAnimationLayer layer;
        // 子状态机数组，有则添加，无则忽略
        [FormerlySerializedAs("subStateMachineName")] public List<string> subStateMachineNames;
        /// 动画状态名称，不包含层级和子状态机
        public string animationStateName;
        /// 动画hash，由状态名自动计算获取
        public int animationHash;
        /// 动画类型
        public EAnimationType animationType;
        /// 是否循环
        public bool loop;
        /// 是否作为当前层的默认状态，每层只能有一个默认状态
        public bool isDefault;
        /// 上一个动画过渡到当前配置的动画的绝对时间，传入到CrossFadeAPI
        public float transitionInTime = 0.1f;
        /// 当前状态动画的起始播放偏移
        public float normalizedTimeOffset;
        /// 忽略的动画类型，决定当前动画不能被这些类型打断
        public List<AnimationIgnore> ignores;
        /// 非循环动画结束后是否切换为该层的默认动画状态，true则切换，false则停留在当前状态，循环动画忽略该字段
        public bool isSwitchDefault;
    }
}
