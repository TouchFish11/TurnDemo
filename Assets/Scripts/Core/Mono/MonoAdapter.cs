using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Log;
using Core.Mono.MonoFunction;
using Core.Singleton;
using UnityEngine;

namespace Core.Mono
{
    /// <summary>
    /// Mono适配器
    /// </summary>
    public class MonoAdapter : SingletonAutoMono<MonoAdapter>, IMonoAdapter, IInitializable
    {
        private List<IAwakable> awakables = new();
        private List<IEnable> enables = new();
        private List<IStartable> startables = new();
        private List<IDisable> disables = new();
        private List<IDestroyable> destroyables = new();

        
        
        private List<Action> _fixedUpdates = new();
        private List<Action> _updates = new();
        private List<Action> _lateUpdates = new();

        /// <summary>
        /// 应用程序退出事件
        /// </summary>
        public event Func<Task> OnAppQuit;
        
        /// <summary>
        /// 应用程序暂停事件
        /// </summary>
        public event Func<bool, Task> OnAppPause; 
        
        /// <summary>
        /// 应用程序焦点事件
        /// </summary>
        public event Func<bool, Task> OnAppFocus;

        public int Priority => -1;

        public Task InitAsync()
        {
            return Task.CompletedTask;
        }
        
        private void Awake()
        {
            for (var i = 0; i < awakables.Count; ++i)
            {
                awakables[i].Awake();
            }
        }

        private void OnEnable()
        {
            
        }

        private void Start()
        {
            
        }

        public new Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return base.StartCoroutine(coroutine);
        }

        public void AddFixedUpdateListener(Action fixedUpdateFun)
        {
            if (fixedUpdateFun == null)
            {
                return;
            }
            _fixedUpdates.Add(fixedUpdateFun);
        }

        public void AddUpdateListener(Action updateFun)
        {
            if (updateFun == null)
            {
                return;
            }
            _updates.Add(updateFun);
        }

        public void AddLateUpdateListener(Action lateUpdateFun)
        {
            if (lateUpdateFun == null)
            {
                return;
            }
            _lateUpdates.Add(lateUpdateFun);
        }

        public void RemoveFixedUpdateListener(Action fixedUpdateFun)
        {
            if (fixedUpdateFun == null)
            {
                return;
            }
            _fixedUpdates?.Remove(fixedUpdateFun);
        }

        public void RemoveUpdateListener(Action updateFun)
        {
            if (updateFun == null)
            {
                return;
            }
            _updates?.Remove(updateFun);
        }
        
        public void RemoveLateUpdateListener(Action lateUpdateFun)
        {
            if (lateUpdateFun == null)
            {
                return;
            }
            _lateUpdates?.Remove(lateUpdateFun);
        }

        private void FixedUpdate()
        {
            for (var i = 0; i < _fixedUpdates.Count; ++i)
            {
                _fixedUpdates[i]?.Invoke();
            }
        }

        private void Update()
        {
            for (var i = 0; i < _updates.Count; ++i)
            {
                _updates[i]?.Invoke();
            }
        }

        private void LateUpdate()
        {
            for (var i = 0; i < _lateUpdates.Count; ++i)
            {
                _lateUpdates[i]?.Invoke();
            }
        }

        private void OnDisable()
        {
            
        }

        private async void OnApplicationQuit()
        {
            try
            {
                if (OnAppQuit == null)
                {
                    return;
                }
                
                await OnAppQuit();
                OnAppQuit = null;
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(MonoAdapter)}.{nameof(OnApplicationQuit)}：{e.Message}，{e.StackTrace}");
            }
        }

        private async void OnApplicationPause(bool pauseStatus)
        {
            try
            {
                if (OnAppPause == null)
                {
                    return;
                }
                
                await OnAppPause(pauseStatus);
                OnAppPause = null;
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(MonoAdapter)}.{nameof(OnApplicationPause)}：{e.Message}，{e.StackTrace}");
            }
        }

        private async void OnApplicationFocus(bool hasFocus)
        {
            try
            {
                if (OnAppFocus == null)
                {
                    return;
                }
                
                await OnAppFocus(hasFocus);
                OnAppFocus = null;
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(MonoAdapter)}.{nameof(OnApplicationFocus)}：{e.Message}，{e.StackTrace}");
            }
        }

        protected override void OnDestroy()
        {
            awakables.Clear();
            awakables = null;

            _fixedUpdates.Clear();
            _updates.Clear();
            _lateUpdates.Clear();
            _fixedUpdates = null;
            _updates = null;
            _lateUpdates = null;
            
            base.OnDestroy();
        }
    }
}
