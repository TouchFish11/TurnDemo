using HotUpdate.Base.Animation;
using HotUpdate.Base.Component;
using HotUpdate.Game.Battle.Core;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Animation.Component
{
    /// <summary>
    /// 战斗动画组件
    /// 负责处理战斗实体（玩家/怪物）的各类战斗相关动画状态切换，
    /// 监听技能选择、技能释放等战斗事件，并根据事件触发对应动画
    /// </summary>
    [ComponentId(typeof(BattleAnimationComponent))]
    public class BattleAnimationComponent : BattleComponent, IBattleAnimationComponent
    {
        private AnimatorComponent _animatorComponent;

        public Animator Animator => _animatorComponent.Animator;
        
        public string AnimationState { get; private set; }

        protected override void OnBattleInit()
        {
            _animatorComponent = BattleEntity.GetComponent<AnimatorComponent>();
        }

        /// <summary>
        /// 设置通用动画播放状态
        /// </summary>
        /// <param name="type">要切换的动画类型</param>
        public void SetCommonState(EAnimationType type)
        {
            _animatorComponent.PlayCommon(type);
        }

        public void SetSkillState(string stateName)
        {
            // 更新当前动画类型
            AnimationState = stateName;
            _animatorComponent.Play(stateName);
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
        
        protected override void OnBattleDestroy()
        {
            _animatorComponent = null;
        }
    }
}