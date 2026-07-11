using System;
using System.Collections.Generic;
using System.Threading;
using Core.Log;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace Core.UI
{
    public class ReactiveProperty<T> : IDisposable
    {
        private T _value;
        private List<Action<T>> _onValueChangeds;
        private IDisposable _preProperty;
        private bool _isDisposed;
        private bool _isNotifying;

#if UNITY_EDITOR
        private int _notifyDepth;
#endif
        
        public T Value
        {
            get => _value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_value, value))
                {
                    Logger.LogDebug(ELogTags.Reactive, $"相等性判断, {_value}");
                    return;
                }
                
                // 如果当前正在通知，说明这是一次重入调用，我们不立刻再次通知，
                // 而是直接返回（值已经更新，但不会触发新的通知，从而终止循环）。
                // if (_isNotifying)
                // {
                //     Logger.Log($"[{nameof(ReactiveProperty<T>)}]: 重入调用, {_value}");
                //     return;
                // }

                _value = value;
                Invoke(_value);
            }
        }

        public ReactiveProperty(T initialValue = default)
        {
            _value = initialValue;
            _onValueChangeds = new List<Action<T>>();
            _preProperty = null;
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="invokeImmediately"></param>
        /// <returns></returns>
        public IDisposable Subscribe(Action<T> listener, bool invokeImmediately = true)
        {
            if(_isDisposed)
                throw new ObjectDisposedException(nameof(ReactiveProperty<T>));
            
            if(listener == null)
                throw new ArgumentNullException(nameof(listener));
            
            _onValueChangeds.Add(listener);

            // 立即调用一次
            if (invokeImmediately)
                listener(_value);

            return new Subscription(this, listener);
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="listener"></param>
        public void Unsubscribe(Action<T> listener)
        {
            if(_isDisposed)
                throw new ObjectDisposedException(nameof(ReactiveProperty<T>));
            
            _onValueChangeds.Remove(listener);
        }
        
        /// <summary>
        /// 强制通知
        /// </summary>
        public void ForceNotify()
        {
            Invoke(_value);
        }

        /// <summary>
        /// 绑定到 Unity 生命周期
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="listener"></param>
        /// <param name="invokeImmediately"></param>
        /// <returns></returns>
        public IDisposable Bind(MonoBehaviour owner, Action<T> listener, bool invokeImmediately = true)
        {
            if (!owner)
                throw new ArgumentNullException(nameof(owner));
            
            var subscription = (Subscription)Subscribe(listener, invokeImmediately);

            var token = owner.destroyCancellationToken;
            var registration = token.Register(subscription.Dispose);
            subscription.SetCancelToken(registration);

            return subscription;
        }

        /// <summary>
        /// 选择映射格式，用于UI
        /// </summary>
        /// <param name="selector"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public ReactiveProperty<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            var result = new ReactiveProperty<TResult>(selector(_value));

            Subscribe(v =>
            {
                result.Value = selector(v);
            });
            
            // 链接为上一个节点，用于链式销毁
            _preProperty = result;
            
            return result;
        }

        /// <summary>
        /// 执行所有回调
        /// </summary>
        /// <param name="value"></param>
        private void Invoke(T value)
        {
            if (_onValueChangeds == null || _onValueChangeds.Count == 0)
                return;

#if UNITY_EDITOR
            _notifyDepth++;
            if (_notifyDepth > 20)
            {
                Logger.LogError(ELogTags.Reactive, $"检测到可能的无限递归，深度 {_notifyDepth}，请检查订阅逻辑！");
                _notifyDepth--;
                return; // 熔断
            }
#endif
            
            _isNotifying = true;
            // 复制一份避免回调中修改列表导致异常
            var snapshot = new List<Action<T>>(_onValueChangeds);
            foreach (var listener in snapshot)
            {
                try
                {
                    listener(value);
                }
                catch (Exception e)
                {
                    Logger.LogError(ELogTags.Reactive, $"[{nameof(ReactiveProperty<T>)}]: callBack invoke error, {e.Message}");
                }
            }

            _isNotifying = false;
#if UNITY_EDITOR
            _notifyDepth--;
#endif
        }
        
        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            _onValueChangeds?.Clear();
            _onValueChangeds = null;
            
            _preProperty?.Dispose();
            _preProperty = null;
        }

        internal class Subscription : IDisposable
        {
            private ReactiveProperty<T> _source;
            private Action<T> _listener;
            private CancellationTokenRegistration _tokenRegistration;

            public Subscription(ReactiveProperty<T> source, Action<T> listener)
            {
                _source = source;
                _listener = listener;
            }

            public void SetCancelToken(CancellationTokenRegistration tokenRegistration)
            {
                _tokenRegistration = tokenRegistration;
            }

            public void Dispose()
            {
                if (_source != null && _listener != null)
                {
                    _source.Unsubscribe(_listener);
                    _source = null;
                    _listener = null;
                    _tokenRegistration.Dispose();
                }
            }
        }
    }
}