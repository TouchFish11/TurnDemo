using HotUpdate.Base.Enums;
using HotUpdate.Game.Animation.Component;

namespace HotUpdate.Game.Animation
{
    public class NormalAnimationComponentCore : AnimationComponentCore<NormalAnimationComponent>
    {
        public override E_AnimationType CurrentAnimationType { get; set; } = E_AnimationType.None;

        public override void SetAnimationState(int type)
        {
            var animationType = (E_AnimationType)type;
            switch (animationType)
            {
                case E_AnimationType.Idle:
                    // 切换为待机动画：设置跑步参数为false
                    AnimatorComponent.Animator.SetBool(AnimationParameter.IsRunHash, false);
                    break;
                case E_AnimationType.Run:
                    // 切换为跑步动画：设置跑步参数为true
                    AnimatorComponent.Animator.SetBool(AnimationParameter.IsRunHash, true);
                    break;
                case E_AnimationType.NormalAttack:
                    // 触发普通攻击动画：调用攻击触发参数
                    AnimatorComponent.Animator.SetTrigger(AnimationParameter.NormalAtkTirggerHash);
                    break;
            }
            // 更新当前动画类型记录
            CurrentAnimationType = animationType;
        }
    }
}
