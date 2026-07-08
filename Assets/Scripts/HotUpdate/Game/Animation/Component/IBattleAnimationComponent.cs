using System.Collections;

namespace HotUpdate.Game.Animation.Component
{
    /// <summary>
    /// 战斗动画组件接口
    /// </summary>
    public interface IBattleAnimationComponent : IAnimationComponent
    {
        /// <summary>
        /// 当前播放的动画状态名称
        /// </summary>
        string AnimationState { get; }
        
        /// <summary>
        /// 等待播放到指定技能动画状态
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="targetProgress"></param>
        IEnumerator PlayToTarget(string stateName, float targetProgress = 0);
    }
}
