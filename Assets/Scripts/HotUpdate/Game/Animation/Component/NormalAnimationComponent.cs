using HotUpdate.Base.Component;
using HotUpdate.Base.Enums;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Inputs;
using UnityEngine;

namespace HotUpdate.Game.Animation.Component
{
    /// <summary>
    /// 常规动画组件
    /// 负责处理角色基础的移动、普通攻击等常规动画逻辑
    /// </summary>
    [ComponentId(typeof(NormalAnimationComponent))]
    public class NormalAnimationComponent : AnimationComponent, INormalAnimationComponent
    {
        /// <summary>
        /// 当前播放的动画类型
        /// </summary>
        protected override E_AnimationType CurrentAnimationType { get; set; } = E_AnimationType.None;
        
        protected override void OnAnimationInit()
        {
            // 注册输入组件的事件监听：移动输入变化、鼠标左键点击（普通攻击）
            EntityObject.GetComponent<IInputComponent>().OnKeyInputChanged += OnMove;
            EntityObject.GetComponent<IInputComponent>().OnMouseLeftClick += OnAttack;
            
            // 初始化时将战斗层、技能层动画权重设为0，优先使用基础动画层
            animatorComponent.Animator.SetLayerWeight(animatorComponent.Animator.GetLayerIndex(AnimationUtility.Battle_Layer_Name), 0);
            animatorComponent.Animator.SetLayerWeight(animatorComponent.Animator.GetLayerIndex(AnimationUtility.Skill_Layer_Name), 0);
        }

        /// <summary>
        /// 设置动画器引用
        /// 预留方法，用于外部设置Animator组件引用
        /// </summary>
        /// <param name="animator">目标Animator组件</param>
        public void SetAnimator(Animator animator)
        {

        }

        /// <summary>
        /// 设置动画播放状态
        /// 根据指定的动画类型切换对应的动画参数
        /// </summary>
        /// <param name="type"></param>
        public override void SetAnimationState(int type)
        {
            E_AnimationType animationType = (E_AnimationType)type;
            switch (animationType)
            {
                case E_AnimationType.Idle:
                    // 切换为待机动画：设置跑步参数为false
                    animatorComponent.Animator.SetBool(animationArg.IsRunHash, false);
                    break;
                case E_AnimationType.Run:
                    // 切换为跑步动画：设置跑步参数为true
                    animatorComponent.Animator.SetBool(animationArg.IsRunHash, true);
                    break;
                case E_AnimationType.NormalAttack:
                    // 触发普通攻击动画：调用攻击触发参数
                    animatorComponent.Animator.SetTrigger(animationArg.NormalAtkTirggerHash);
                    break;
            }
            // 更新当前动画类型记录
            CurrentAnimationType = animationType;
        }

        /// <summary>
        /// 移动输入事件回调方法
        /// 根据输入方向判断切换待机/跑步动画
        /// </summary>
        /// <param name="inputDir">输入的移动方向向量</param>
        private void OnMove(Vector3 inputDir)
        {
            // 输入方向非零则播放跑步动画，否则播放待机动画
            SetAnimationState((int)(inputDir != Vector3.zero ? E_AnimationType.Run : E_AnimationType.Idle));
        }

        /// <summary>
        /// 普通攻击输入事件回调方法
        /// 触发普通攻击动画播放
        /// </summary>
        private void OnAttack()
        {
            SetAnimationState((int)E_AnimationType.NormalAttack);
        }
        
        
        protected override void OnDestroyBase()
        {
            
        }
    }
}