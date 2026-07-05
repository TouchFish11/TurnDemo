using HotUpdate.Base.Animation;
using HotUpdate.Base.Component;
using HotUpdate.Base.Enums;
using HotUpdate.Game.Inputs;
using UnityEngine;

namespace HotUpdate.Game.Animation.Component
{
    /// <summary>
    /// 常规动画组件
    /// 负责处理角色基础的移动、普通攻击等常规动画逻辑
    /// </summary>
    [ComponentId(typeof(NormalAnimationComponent))]
    [ComponentCore(typeof(NormalAnimationComponentCore))]
    public class NormalAnimationComponent : BaseComponent, IAnimationComponent
    {
        private NormalAnimationComponentCore _normalAnimationComponentCore;

        public Animator Animator => _normalAnimationComponentCore.GetAnimator();
        
        public AnimationParameter Parameter => _normalAnimationComponentCore.GetParameter();
        
        protected override void OnInit()
        {
            _normalAnimationComponentCore = (NormalAnimationComponentCore)ComponentCore;
            var animatorComponent = _normalAnimationComponentCore.AnimatorComponent;
            
            // 注册输入组件的事件监听：移动输入变化、鼠标左键点击（普通攻击）
            EntityObject.GetComponent<InputComponent>().AddKeyInputChangedListener(OnMove);
            EntityObject.GetComponent<InputComponent>().AddMouseLeftClickListener(OnAttack);
            
            // 初始化时将战斗层、技能层动画权重设为0，优先使用基础动画层
            animatorComponent.SetLayerWeight(EAnimationLayer.BattleLayer, 0);
            animatorComponent.SetLayerWeight(EAnimationLayer.SkillLayer, 0);
        }

        public void SetAnimationState(int type)
        {
            _normalAnimationComponentCore.SetAnimationState(type);
        }
        
        public AnimatorStateInfo GetCurrentAnimatorStateInfo(string layerName)
        {
            return _normalAnimationComponentCore.GetCurrentAnimatorStateInfo(layerName);
        }

        /// <summary>
        /// 移动输入事件回调方法
        /// 根据输入方向判断切换待机/跑步动画
        /// </summary>
        /// <param name="inputDir">输入的移动方向向量</param>
        private void OnMove(Vector3 inputDir)
        {
            // 输入方向非零则播放跑步动画，否则播放待机动画
            _normalAnimationComponentCore.SetAnimationState((int)(inputDir != Vector3.zero ? E_AnimationType.Run : E_AnimationType.Idle));
        }

        /// <summary>
        /// 普通攻击输入事件回调方法
        /// 触发普通攻击动画播放
        /// </summary>
        private void OnAttack()
        {
            _normalAnimationComponentCore.SetAnimationState((int)E_AnimationType.NormalAttack);
        }
        
        protected override void OnBaseDestroy()
        {
            _normalAnimationComponentCore = null;
        }
    }
}