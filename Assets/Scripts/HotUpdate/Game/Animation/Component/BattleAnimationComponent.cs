using System.Collections;
using HotUpdate.Base.Animation;
using HotUpdate.Base.Component;
using HotUpdate.Base.Utility;
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
        public AnimatorState Play(EAnimationType type)
        {
            return _animatorComponent.PlayCommon(type);
        }

        public IEnumerator PlayToTarget(string stateName, float targetProgress = 0)
        {
            var state = _animatorComponent.Play(stateName);
            if (state != null)
            {
                var config = state.Config;
                var layerName = AnimationLayer.LayerEnumToName(config.layer);
                // 等待切换到指定状态
                yield return new WaitUntil(() => GetCurrentAnimatorStateInfo(layerName).fullPathHash == config.animationHash && !Animator.IsInTransition((int)config.layer));
                // 更新当前动画类型
                AnimationState = stateName;
                // 等待状态动画播放到指定进度
                yield return new WaitUntil(() => GetCurrentAnimatorStateInfo(layerName).normalizedTime >= targetProgress);
            }
        }

        public IEnumerator WaitForPlay(string stateName)
        {
            if (!_animatorComponent.TryGetState(stateName, out var state))
            {
                Logger.LogError($"[{nameof(BattleAnimationComponent)}]: wait for {stateName} state fail, not found the state");
                yield break;
            }
            
            var config = state.Config;
            var layerName = AnimationLayer.LayerEnumToName(config.layer);
            // 等待状态动画播放到指定进度
            yield return new WaitUntil(() => GetCurrentAnimatorStateInfo(layerName).normalizedTime >= 0.9);
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