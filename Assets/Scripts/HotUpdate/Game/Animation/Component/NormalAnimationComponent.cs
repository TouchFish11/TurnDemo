using HotUpdate.Base.Animation;
using HotUpdate.Base.Component;
using HotUpdate.Game.Inputs;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Animation.Component
{
    /// <summary>
    /// 常规动画组件
    /// 负责处理角色基础的移动、普通攻击等常规动画逻辑
    /// </summary>
    [ComponentId(typeof(NormalAnimationComponent))]
    public class NormalAnimationComponent : BaseComponent, IAnimationComponent
    {
        private AnimatorComponent _animatorComponent;

        public Animator Animator => _animatorComponent.Animator;
        
        public string AnimationState { get; private set; }
        
        protected override void OnInit()
        {
            _animatorComponent = EntityObject.GetComponent<AnimatorComponent>();
            _animatorComponent.AddAnimationFinished(OnAttackFinished);
            
            // 注册输入组件的事件监听：移动输入变化、鼠标左键点击（普通攻击）
            EntityObject.GetComponent<InputComponent>().AddKeyInputChangedListener(OnMove);
            EntityObject.GetComponent<InputComponent>().AddMouseLeftClickListener(OnAttack);
        }

        public void SetCommonState(EAnimationType type)
        {
            // 更新当前动画类型
            AnimationState = type.ToString();
            _animatorComponent.PlayCommon(type);
        }
        
        public AnimatorStateInfo GetCurrentAnimatorStateInfo(string layerName)
        {
            if (_animatorComponent)
            {
                var animator = _animatorComponent.Animator;
                return animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex(layerName));
            }
            
            Logger.LogError("动画控制器为null");
            return new AnimatorStateInfo();
        }

        private Vector3 lastInput;

        /// <summary>
        /// 移动输入事件回调方法
        /// 根据输入方向判断切换待机/跑步动画
        /// </summary>
        /// <param name="inputDir">输入的移动方向向量</param>
        private void OnMove(Vector3 inputDir)
        {
            // 输入方向非零则播放跑步动画，否则播放待机动画
            if (inputDir == Vector3.zero && lastInput != Vector3.zero)
            {
                _animatorComponent.PlayCommon(EAnimationType.Idle);
                lastInput = inputDir;
            }
            else if(inputDir != Vector3.zero && lastInput == Vector3.zero)
            {
                _animatorComponent.PlayCommon(EAnimationType.Run);
                lastInput = inputDir;
            }
        }
    
        /// <summary>
        /// 普通攻击输入事件回调方法
        /// 触发普通攻击动画播放
        /// </summary>
        private void OnAttack()
        {
            _animatorComponent.PlayCommon(EAnimationType.WorldAttack);
        }

        private void OnAttackFinished(AnimationConfig config)
        {
            if (lastInput != Vector3.zero)
            {
                _animatorComponent.PlayCommon(EAnimationType.Run);
            }
        }
    }
}