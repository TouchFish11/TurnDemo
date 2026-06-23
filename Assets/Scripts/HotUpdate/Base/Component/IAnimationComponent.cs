using HotUpdate.Base.Animation;
using UnityEngine;

namespace HotUpdate.Base.Component
{
    public interface IAnimationComponent
    {
        /// <summary>
        /// 设置动画类型
        /// </summary>
        /// <param name="type"></param>
        void SetAnimationState(int type);

        /// <summary>
        /// 获取Animator
        /// </summary>
        /// <returns></returns>
        Animator GetAnimator();

        /// <summary>
        /// 获取动画参数
        /// </summary>
        /// <returns></returns>
        AnimationParameter GetParameter();

        /// <summary>
        /// 获取当前动画状态信息
        /// </summary>
        /// <returns></returns>
        AnimatorStateInfo GetCurrentAnimatorStateInfo(string layerName);
    }
}
