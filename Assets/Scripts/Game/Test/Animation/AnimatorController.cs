using System.Collections.Generic;
using Game.Test.Inputs;
using UnityEngine;

namespace Game.Test.Animation
{
    /// <summary>
    /// 动画控制器
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimatorController : MonoBehaviour
    {
        [SerializeField] private List<AnimationConfig> animationConfigs;
        private readonly Dictionary<int, AnimationConfig> _currentConfigs = new();
        private Animator _animator;
        
        public bool IsAttacking 
        {
            get 
            {
                var info = _animator.GetCurrentAnimatorStateInfo((int)EAnimationLayer.ShootLayer);
                return info.IsName(nameof(EAnimationType.Attack)) && info.normalizedTime < 1f;
            }
        }

        public bool IsDashing
        {
            get
            {
                var info = _animator.GetCurrentAnimatorStateInfo((int)EAnimationLayer.BaseLayer);
                return info.IsName(nameof(EAnimationType.Dash)) && info.normalizedTime < 1f;
            }
        }
        
        private void Awake()
        {
            _animator = this.GetComponent<Animator>();

            var defaultConfig = ScriptableObject.CreateInstance<AnimationConfig>();
            defaultConfig.ignores = new List<AnimationIgnore>();
            _currentConfigs.Add((int)EAnimationLayer.BaseLayer, defaultConfig);
            _currentConfigs.Add((int)EAnimationLayer.ShootLayer, defaultConfig);
            Play(EAnimationType.Idle);
        }

        private void Start()
        {
            InputManager.Instance.OnMove += OnMove;
            InputManager.Instance.OnAttack += OnAttack;
            InputManager.Instance.OnDash += OnDash;
            InputManager.Instance.OnJump += OnJump;
        }

        private void OnJump()
        {
            Play(EAnimationType.Jump);
        }

        private void OnDash()
        {
            Play(EAnimationType.Dash);
        }

        private void OnAttack(bool isDown)
        {
            Play(isDown ? EAnimationType.Attack : EAnimationType.Null);
        }

        private void OnMove(Vector2 vector2)
        {
            var config = _currentConfigs[(int)EAnimationLayer.BaseLayer];
            if (config.animationType == EAnimationType.Run && vector2 == Vector2.zero)
            {
                Play(EAnimationType.Idle);
            }
            else if (config.animationType == EAnimationType.Idle && vector2 != Vector2.zero)
            {
                Play(EAnimationType.Run);
            }
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
            if (!TryGetConfig(type, out var config))
            {
                return;
            }

            if (!_currentConfigs.TryGetValue((int)config.layer, out _)) return;
            // 切换动画
            PlayInternal(config.animationHash, !config ? 0 : config.transitionInTime, (int)config.layer, 0); 
            // 更新当前层级配置
            _currentConfigs[(int)config.layer] = config;
            RefreshIgnores(config);
        }

        internal void PlayInternal(int stateHashName, float normalizedTransitionDuration, int layer, float normalizedTimeOffset)
        {
            _animator.CrossFade(stateHashName, normalizedTransitionDuration, layer, normalizedTimeOffset);
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
                    ignore.Update(Time.deltaTime);
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
                    // 当前动画不能被打断
                    if (!currentConfigIgnore.IgnoreOver && (currentConfigIgnore.ignoreType & type) != 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 尝试获取动画配置
        /// </summary>
        /// <param name="type"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool TryGetConfig(EAnimationType type, out AnimationConfig config)
        {
            var cacheConfig = animationConfigs.Find(config => config.animationType == type);
            if (cacheConfig)
            {
                config = cacheConfig;
                return true;
            }
            config = null;
            return false;
        }

        private void Update()
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
                var stateInfo = _animator.GetCurrentAnimatorStateInfo(currentLayer);
                if (currentConfig.loop || stateInfo.fullPathHash != currentConfig.animationHash || !(stateInfo.normalizedTime >= 1f)) continue;
                Debug.Log($"当前动画：{currentConfig.animationStateName}：{stateInfo.fullPathHash}播放完成");
                
                // 是否存在下一个连携的动画
                if (currentConfig.nextAnimConfig)
                {
                    PlayInternal(currentConfig.nextAnimConfig.animationHash, currentConfig.nextAnimConfig.transitionInTime, (int)currentConfig.nextAnimConfig.layer, 0);
                    _currentConfigs[currentLayer] = currentConfig.nextAnimConfig;
                    RefreshIgnores(currentConfig.nextAnimConfig);
                }
                else
                {
                    Play(currentLayer == 0 ? EAnimationType.Idle : EAnimationType.Null);
                }
            }
        }
    }
}
