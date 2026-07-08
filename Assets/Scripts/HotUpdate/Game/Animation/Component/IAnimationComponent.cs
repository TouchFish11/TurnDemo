using HotUpdate.Base.Animation;
using UnityEngine;

namespace HotUpdate.Game.Animation.Component
{
    /// <summary>
    /// 动画组件接口
    /// </summary>
    public interface IAnimationComponent
    {
        /// <summary>
        /// Unity动画控制器
        /// </summary>
        Animator Animator { get; }
        
        /// <summary>
        /// 获取当前动画状态信息
        /// </summary>
        /// <returns></returns>
        AnimatorStateInfo GetCurrentAnimatorStateInfo(string layerName);
        
        /// <summary>
        /// 设置通用动画播放状态
        /// </summary>
        /// <param name="type">要切换的动画类型</param>
        AnimatorState Play(EAnimationType type);
    }
}
