using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Mono;
using Core.Serialize.Json;
using Core.Utility;
using HotUpdate.Base.Animation;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Base.Component
{
    /// <summary>
    /// 动画组件逻辑
    /// </summary>
    public class AnimatorComponentCore : ComponentCore<AnimatorComponent>
    {
        [Inject] private IJsonManager _jsonManager;
        [Inject] private IMonoAdapter _monoAdapter;

        /// 默认的空占位配置
        private static readonly AnimationConfig DefaultConfig = new();
        
        // 动画配置缓存
        private readonly Dictionary<EAnimationType, AnimationConfig> _animationConfigs = new();
        
        // 当前使用的动画配置缓存
        private readonly Dictionary<int, AnimationConfig> _currentConfigs = new()
        {
            {(int)EAnimationLayer.BaseLayer, DefaultConfig},
            {(int)EAnimationLayer.BattleLayer, DefaultConfig},
            {(int)EAnimationLayer.SkillLayer, DefaultConfig},
        };

        protected override void OnInit()
        {
            // 读取动画配置
            using var handle = GameAsset.LoadAsset<TextAsset>(AssetKeys.PlayerConfigCollection);
            var collection = _jsonManager.FromJson<AnimationConfigCollection>(handle.Asset.text, settings: NewtonsoftJsonUtility.SerializerSettings);
            foreach (var config in collection.animationConfigs)
            {
                _animationConfigs.Add(config.animationType, config);
            }
            _monoAdapter.AddUpdateListener(Tick); 
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="type"></param>
        /// <returns>播放成功返回true，反正false</returns>
        public void Play(EAnimationType type)
        {
            // 是否忽略该类型动画
            if (IsIgnore(type))
            {
                return;
            }

            // 是否存在该类型动画
            if (!_animationConfigs.TryGetValue(type, out var config))
                return;

            // 检查当前层级是否存在
            if (!_currentConfigs.TryGetValue((int)config.layer, out _)) 
                return;
            
            // 切换动画
            PlayInternal(config.animationHash, config.transitionInTime, (int)config.layer, 0); 
            // 更新当前层级配置
            _currentConfigs[(int)config.layer] = config;
            RefreshIgnores(config);
        }
        
        internal void PlayInternal(int stateHashName, float normalizedTransitionDuration, int layer, float normalizedTimeOffset)
        {
            Component.Animator.CrossFade(stateHashName, normalizedTransitionDuration, layer, normalizedTimeOffset);
        }

        /// <summary>
        /// 更新忽略时间
        /// </summary>
        private void UpdateIgnores()
        {
            foreach (var configs in _currentConfigs.Values)
            {
                for (var i = configs.ignores.Count - 1; i >= 0; i--)
                {
                    var ignore = configs.ignores[i];
                    ignore.Update(TimeUtil.DeltaTime);
                }
            }
        }
        
        /// <summary>
        /// 刷新忽略时间
        /// </summary>
        private static void RefreshIgnores(AnimationConfig config)
        {
            foreach (var currentConfigIgnore in config.ignores)
            {
                currentConfigIgnore.Reset();
            }
        }
        
        /// <summary>
        /// 是否忽略该动画类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns>true为忽略，false为不忽略</returns>
        public bool IsIgnore(EAnimationType type)
        {
            foreach (var config in _currentConfigs.Values)
            {
                foreach (var currentConfigIgnore in config.ignores)
                {
                    // 忽略未结束且当前动画忽略参数的动画类型，则当前动画不能被参数动画类型打断
                    if (!currentConfigIgnore.IgnoreOver && currentConfigIgnore.ignoreType == type)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        public void Tick()
        {
            UpdateIgnores();
            
            // 当前动画是否播放完毕
            CheckAnimationFinished();
        }
        
        /// <summary>
        /// 检测非循环动画播放完成
        /// </summary>
        private void CheckAnimationFinished()
        {
            foreach (var (currentLayer, currentConfig) in _currentConfigs)
            {
                // 获取当前层级播放的动画信息
                var stateInfo = Component.Animator.GetCurrentAnimatorStateInfo(currentLayer);
                if (currentConfig.loop || stateInfo.fullPathHash != currentConfig.animationHash || !(stateInfo.normalizedTime >= 1f)) continue;
                Logger.Log($"当前动画：{currentConfig.animationStateName}：{stateInfo.fullPathHash}播放完成");
                
                // 是否存在下一个连携的动画
                if (currentConfig.nextAnimConfig != null)
                {
                    PlayInternal(currentConfig.nextAnimConfig.animationHash, currentConfig.nextAnimConfig.transitionInTime, (int)currentConfig.nextAnimConfig.layer, 0);
                    _currentConfigs[currentLayer] = currentConfig.nextAnimConfig;
                    RefreshIgnores(currentConfig.nextAnimConfig);
                }
                else
                {
                    Play(currentLayer == 0 ? EAnimationType.Idle : EAnimationType.None);
                }
            }
        }

        protected override void OnDispose()
        {
            _monoAdapter.RemoveUpdateListener(Tick);
            _animationConfigs.Clear();
            _currentConfigs.Clear();
        }
    }
}
