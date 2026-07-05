using HotUpdate.Base.Animation;
using UnityEngine;

namespace HotUpdate.Base.Component
{
    public interface IAnimationComponent : IComponent
    {
        /// <summary>
        /// Unity动画控制器
        /// </summary>
        Animator Animator { get; }
        
        /// <summary>
        /// 动画参数
        /// </summary>
        AnimationParameter Parameter { get; }
        
        /// <summary>
        /// 获取当前动画状态信息
        /// </summary>
        /// <returns></returns>
        AnimatorStateInfo GetCurrentAnimatorStateInfo(string layerName);
    }
}
