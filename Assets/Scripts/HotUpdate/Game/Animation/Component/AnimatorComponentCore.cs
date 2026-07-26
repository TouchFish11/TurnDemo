using System;
using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Mono;
using Core.Serialize.Json;
using HotUpdate.Base.Animation;
using HotUpdate.Base.ECModule;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Utility;
using UnityEngine;

namespace HotUpdate.Game.Animation.Component
{
    /// <summary>
    /// 动画组件逻辑
    /// </summary>
    public class AnimatorComponentCore : ComponentCore<AnimatorComponent>
    {
        [Inject] private IJsonManager _jsonManager;
        [Inject] private IMonoAdapter _monoAdapter;
        
        // 当前动画状态机的所有层级对象缓存
        private readonly List<AnimatorLayer> _layers = new();
        // 当前所有层级的快照缓存
        private readonly List<AnimatorLayer> _currentLayersSnapshot = new();
        
        public event Action<AnimationConfig> OnAnimationFinished;
        
        protected override void OnInit()
        {
            _monoAdapter.AddUpdateListener(Tick); 
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        public void InitConfigs()
        {
            var assetKey = BattleUtility.GetAnimatorControllerAssetKeyByType((IBattleEntityObject)Component.EntityObject);
            using var handle = GameAsset.LoadAsset<RuntimeAnimatorController>(assetKey);
            Component.Animator.runtimeAnimatorController = handle.Asset;
                
            // 读取动画配置
            var collectionJson = AnimationUtility.GetAnimConfigCollectionJsonByType(Component.EntityObject);
            var collection = _jsonManager.FromJson<AnimationConfigCollection>(collectionJson, settings: NewtonsoftJsonUtility.DefaultSerializerSettings);

            var allConfigs = new List<AnimationConfig>();
            allConfigs.AddRange(collection.commonCollection.animationConfigs);
            allConfigs.AddRange(collection.animationConfigs);
            
            // 层级到动画状态配置映射
            Dictionary<int, List<AnimationConfig>> layerToConfigs = new();
            foreach (var config in allConfigs)
            {
                var layerIndex = (int)config.layer;
                if (layerToConfigs.ContainsKey(layerIndex))
                {
                    layerToConfigs[layerIndex].Add(config);
                }
                else
                {
                    layerToConfigs.Add(layerIndex, new List<AnimationConfig> { config });
                }
            }
            
            var layerCount = Component.Animator.layerCount;
            for (var i = 0; i < layerCount; i++)
            {
                var layer = new AnimatorLayer(i);
                if (layerToConfigs.TryGetValue(i, out var configs))
                {
                    foreach (var config in configs)
                    {
                        var state = new AnimatorState(config);
                        layer.AddState(state.FullPathHash, state);
                    }
                }
                
                _layers.Add(layer);
            }

        }

        /// <summary>
        /// 播放通用动画，技能层动画使用Play方法
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="NotSupportedException">当type为Attack时抛出</exception>
        public AnimatorState PlayCommon(EAnimationType type)
        {
            if (type == EAnimationType.Attack)
                throw new NotSupportedException($"The animation type {type} is not supported.");
            
            // 是否忽略该类型动画
            if (IsIgnore(type))
            {
                return null;
            }
            
            foreach (var animatorLayer in _layers)
            {
                if (!animatorLayer.TryGetState(type, out var state)) 
                    continue;
                
                PlayInternal(state); 
                // 更新当前播放的动画
                animatorLayer.UpdateState(state.FullPathHash);
                return state;
            }

            return null;
        }

        /// <summary>
        /// 播放指定层级指定状态名称的动画
        /// </summary>
        /// <param name="stateName">层级名.状态名</param>
        public AnimatorState Play(string stateName)
        {
            var animationHash = Animator.StringToHash(stateName);
            foreach (var animatorLayer in _layers)
            {
                if (!animatorLayer.TryGetState(animationHash, out var state)) 
                    continue;
                
                // 更新当前播放的动画
                animatorLayer.UpdateState(animationHash);
                PlayInternal(state); 
                return state;
            }

            return null;
        }
        
        internal void PlayInternal(AnimatorState state)
        {
            var config = state.Config;
            Component.Animator.CrossFadeInFixedTime(config.animationHash, config.transitionInTime, (int)config.layer, config.normalizedTimeOffset);
        }

        /// <summary>
        /// 尝试获取指定动画
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool TryGetState(string stateName, out AnimatorState state)
        {
            var animationHash = Animator.StringToHash(stateName);
            foreach (var animatorLayer in _layers)
            {
                if (!animatorLayer.TryGetState(animationHash, out var cacheState))
                    continue;

                state = cacheState;
                return true;
            }

            state = null;
            return false;
        }

        /// <summary>
        /// 更新忽略时间
        /// </summary>
        private void UpdateIgnores()
        {
            foreach (var layer in _layers)
            {
                layer.UpdateIgnores();
            }
        }

        /// <summary>
        /// 是否忽略该动画类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns>true为忽略，false为不忽略</returns>
        public bool IsIgnore(EAnimationType type)
        {
            foreach (var layer in _layers)
            {
                if (layer.IsIgnore(type))
                {
                    return true;
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
            _currentLayersSnapshot.Clear();
            _currentLayersSnapshot.AddRange(_layers);
            foreach (var currentLayer in _currentLayersSnapshot)
            {
                if(currentLayer.CurrentState == null)
                    continue;
                
                var config = currentLayer.CurrentState.Config;
                var layer = (int)config.layer;
                // 获取当前层级播放的动画信息
                var stateInfo = Component.Animator.GetCurrentAnimatorStateInfo(layer);
                if (config.loop || stateInfo.fullPathHash != config.animationHash || !(stateInfo.normalizedTime >= 1f)) 
                    continue;
                
                // 再判断是否切换到当前层的默认动画状态
                if(config.isSwitchDefault)
                {
                    currentLayer.ResetToDefault();
                    PlayInternal(currentLayer.CurrentState);
                }

                // 否则不处理当前层的当前状态，使其停留在当前状态
                OnAnimationFinished?.Invoke(config);
            }
        }

        protected override void OnDispose()
        {
            OnAnimationFinished = null;
            _monoAdapter.RemoveUpdateListener(Tick);
            _currentLayersSnapshot.Clear();
            _layers.Clear();
        }
    }
}
