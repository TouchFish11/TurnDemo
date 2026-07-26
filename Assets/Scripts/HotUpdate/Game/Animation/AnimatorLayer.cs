using System;
using System.Collections.Generic;
using Core.Time;
using HotUpdate.Base.Animation;

namespace HotUpdate.Game.Animation
{
    /// <summary>
    /// Animator状态机层级对象
    /// </summary>
    public class AnimatorLayer
    {
        // 动画配置的状态对象缓存，key是层级+状态名的hash，value是状态对象
        private readonly Dictionary<int, AnimatorState> _hashToStateMap = new();
        // 通用类型到动画状态配置的映射，key是动画类型枚举，value是对应的状态hash
        private readonly Dictionary<EAnimationType, int> _commonTypeToConfigHashMap = new();
        // 当前层的默认状态
        private AnimatorState _defaultState;
        
        /// <summary>
        /// 状态机中的层级索引
        /// </summary>
        public int Layer { get; private set; }
        
        /// <summary>
        /// 当前正在播放的状态
        /// </summary>
        public AnimatorState CurrentState { get; private set; }
        
        public AnimatorLayer(int layerIndex)
        {
            Layer = layerIndex;
        }

        public void AddState(int hash, AnimatorState state)
        {
            if (_defaultState != null && state.Config.isDefault)
                throw new Exception("Only one default state is allowed for each layer.");

            _defaultState ??= state.Config.isDefault ? state : null;
            _hashToStateMap.Add(hash, state);
            _commonTypeToConfigHashMap.TryAdd(state.Config.animationType, hash);
        }

        /// <summary>
        /// 更新层级当前正在播放的状态并重置忽略时间
        /// </summary>
        /// <param name="hash"></param>
        public void UpdateState(int hash)
        {
            if (_hashToStateMap.TryGetValue(hash, out var state))
            {
                foreach (var animationIgnore in state.Config.ignores)
                {
                    animationIgnore.Reset();
                }

                CurrentState = state;
            }
        }

        public bool TryGetState(int hash, out AnimatorState state)
        {
            // 是否存在该动画
            if (!_hashToStateMap.TryGetValue(hash, out var cacheState))
            {
                state = null;
                return false;
            }

            state = cacheState;
            return true;
        }
        
        public bool TryGetState(EAnimationType type, out AnimatorState state)
        {
            if (_commonTypeToConfigHashMap.TryGetValue(type, out var configHash))
                return TryGetState(configHash, out state);
            
            state = null;
            return false;
        }
        
        /// <summary>
        /// 是否忽略进入参数类型的状态
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsIgnore(EAnimationType type)
        {
            if(CurrentState == null)
                return false;
            
            foreach (var animationIgnore in CurrentState.Config.ignores)
            {
                // 忽略未结束且当前动画忽略参数的动画类型，则当前动画不能被参数动画类型打断
                if (!animationIgnore.IgnoreOver && animationIgnore.ignoreType == type)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        public void UpdateIgnores()
        {
            if(CurrentState == null)
                return;
            
            for (var i = CurrentState.Config.ignores.Count - 1; i >= 0; i--)
            {
                var ignore = CurrentState.Config.ignores[i];
                ignore.Update(TimeUtil.DeltaTime);
            }
        }

        /// <summary>
        /// 当前状态重置到默认状态
        /// </summary>
        public void ResetToDefault()
        {
            CurrentState = _defaultState;
        }
    }
}
