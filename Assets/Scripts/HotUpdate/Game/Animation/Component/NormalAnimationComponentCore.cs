using HotUpdate.Base.Animation;

namespace HotUpdate.Game.Animation.Component
{
    /// <summary>
    /// 大世界动画组件
    /// </summary>
    public class NormalAnimationComponentCore : AnimationComponentCore<NormalAnimationComponent>
    {
        public override EAnimationType AnimationType { get; set; }

        public override void SetAnimationState(int type)
        {
            // switch (animationType)
            // {
            //     case E_AnimationType.Idle:
            //         // 切换为待机动画：设置跑步参数为false
            //         AnimatorComponent.Animator.SetBool(AnimationParameter.IsRunHash, false);
            //         break;
            //     case E_AnimationType.Run:
            //         // 切换为跑步动画：设置跑步参数为true
            //         AnimatorComponent.Animator.SetBool(AnimationParameter.IsRunHash, true);
            //         break;
            //     case E_AnimationType.NormalAttack:
            //         // 触发普通攻击动画：调用攻击触发参数
            //         AnimatorComponent.Animator.SetTrigger(AnimationParameter.NormalAtkTirggerHash);
            //         break;
            // }
            // 更新当前动画类型记录
        }
    }
}
