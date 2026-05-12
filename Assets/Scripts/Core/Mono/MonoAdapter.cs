using System;
using System.Collections;
using System.Collections.Generic;
using Core.Mono.MonoFunction;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace Core.Mono
{
    /// <summary>
    /// Mono适配器
    /// </summary>
    public class MonoAdapter : MonoBehaviour, IMonoAdapter
    {
        private List<IAwakable> awakables = new();
        private List<Action> _fixedUpdates = new();
        private List<Action> _updates = new();
        private List<Action> _lateUpdates = new();
        
        private List<IApplicationExitNotify> _applicationExits = new();
        private List<IApplicationPauseNotify> _applicationPauses = new();
        private List<IApplicationFocusNotify> _applicationFocus = new();
        
        private void Awake()
        {
            foreach (var awakable in awakables)
            {
                awakable.Awake();
            }
        }

        private void OnEnable()
        {
            
        }

        private void Start()
        {
            
        }
        
        public void AddApplicationPauseNotify(IApplicationPauseNotify applicationPauseNotify)
        {
            _applicationPauses.Add(applicationPauseNotify);
        }

        public bool RemoveApplicationPauseNotify(IApplicationPauseNotify applicationPauseNotify)
        {
            return _applicationPauses.Remove(applicationPauseNotify);
        }
        
        public void AddApplicationFocusNotify(IApplicationFocusNotify applicationFocusNotify)
        {
            _applicationFocus.Add(applicationFocusNotify);
        }
        
        public bool RemoveApplicationFocusNotify(IApplicationFocusNotify applicationFocusNotify)
        {
            return _applicationFocus.Remove(applicationFocusNotify);
        }
        
        public void AddApplicationExitNotify(IApplicationExitNotify applicationExitNotify)
        {
            _applicationExits.Add(applicationExitNotify);
        }
        
        public void AddApplicationExitNotifies(params IApplicationExitNotify[] applicationExitNotifies)
        {
            _applicationExits.AddRange(applicationExitNotifies);
        }

        public bool RemoveApplicationExitNotify(IApplicationExitNotify applicationExitNotify)
        {
            return _applicationExits.Remove(applicationExitNotify);
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

        private void OnApplicationQuit()
        {
            try
            {
                // 按优先级排序
                _applicationExits.Sort((i1, i2) =>
                {
                    if (i1.QuitPriority > i2.QuitPriority) return 1;
                    if (i1.QuitPriority < i2.QuitPriority) return -1;
                    return 0;
                });
                
                // 依次执行退出逻辑
                foreach (var applicationExitNotify in _applicationExits)
                {
                    applicationExitNotify.OnAppQuit();
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(MonoAdapter)}.{nameof(OnApplicationQuit)}:应用程序退出时逻辑执行错误，{e.Message}");
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            try
            {
                foreach (var applicationPauseNotify in _applicationPauses)
                {
                    applicationPauseNotify.OnAppPause(pauseStatus);
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(MonoAdapter)}.{nameof(OnApplicationPause)}:应用程序暂停/恢复时逻辑执行错误，{e.Message}");
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            try
            {
                foreach (var applicationFocusNotify in _applicationFocus)
                {
                    applicationFocusNotify.OnAppFocus(hasFocus);
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(MonoAdapter)}.{nameof(OnApplicationFocus)}:应用程序聚焦/失焦时逻辑执行错误，{e.Message}");
            }
        }

        protected void OnDestroy()
        {
            awakables.Clear();
            awakables = null;

            _fixedUpdates.Clear();
            _updates.Clear();
            _lateUpdates.Clear();
            _fixedUpdates = null;
            _updates = null;
            _lateUpdates = null;
            
            _applicationExits.Clear();
            _applicationPauses.Clear();
            _applicationFocus.Clear();
            _applicationExits = null;
            _applicationPauses = null;
            _applicationFocus = null;
        }
    }
}
