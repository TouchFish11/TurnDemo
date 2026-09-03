using HotUpdate.Base.Animation;

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
        
        /// <summary>
        /// 非循环动画是否已通知过"播放结束"，防止每帧重复触发
        /// </summary>
        public bool FinishedNotified { get; set; }
        
        public int FullPathHash { get; private set; }
        
        public AnimatorState(AnimationConfig config)
        {
            Config = config;
            FullPathHash = config.animationHash;
        }
    }
}
