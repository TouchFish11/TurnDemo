using System;
using System.Collections.Generic;
using System.Threading;
using Core.DI;
using Core.Exceptions;
using Core.Log;
using Core.Pool;
using Core.Tasks.Awaiter;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace Core.Tasks
{
    /// <summary>
    /// 无返回值自定义任务基类
    /// 围绕 Unity 的 AsyncOperation 系列异步操作，实现了异步等待和取消
    /// </summary>
    internal class AoTask : IPoolData, IDisposable
    {
        [Inject] protected IPoolManager _poolManager;
        // // 多线程锁
        // protected readonly object _lock = new();
        // Unity异步操作对象
        protected AsyncOperation _operation;
        // 取消令牌，用于监听取消请求
        protected CancellationToken _cancellationToken;
        // 取消令牌注册器，用于释放取消监听
        protected CancellationTokenRegistration _cancellationTokenRegistration;
        // Unity上下文
        protected SynchronizationContext _synchronizationContext;
        // 任务完成后的延续回调列表
        protected readonly List<Action> _continuations = new();
        // 任务执行过程中抛出的异常
        protected Exception _exception;
        // 任务是否完成（volatile保证多线程可见性）
        protected volatile bool _isCompleted;
        
        /// <summary>
        /// 任务是否已完成（完成包括成功、失败、取消）
        /// </summary>
        public bool IsCompleted => _isCompleted;
        
        /// <summary>
        /// 初始化任务
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="token"></param>
        public void Init(AsyncOperation operation,  CancellationToken token = default)
        {
            // 若当前上下文为null，抛出异常，自定义任务创建应该规范在主线程
            if (SynchronizationContext.Current == null)
                throw ExceptionHelper.Throw("FTask must be created on main thread");
                
            _operation = operation;
            // 注册原生请求完成的回调
            _operation.completed += RequestCompleted;
            // 保存Unity上下文
            _synchronizationContext = SynchronizationContext.Current;
            // 设置取消令牌
            _cancellationToken = token;
            // 如果取消令牌可取消，则注册取消回调
            if (_cancellationToken.CanBeCanceled)
            {
                // 注册取消回调请求
                _cancellationTokenRegistration = _cancellationToken.Register(RequestCancel, this);
            }
            else
            {
                // 不可取消的令牌，赋值默认注册器
                _cancellationTokenRegistration = default;
            }
        }
        
        /// <summary>
        /// 在取消时触发该回调
        /// </summary>
        /// <param name="state"></param>
        private static void RequestCancel(object state)
        {
            var task = (AoTask)state;
            // 若取消调用在多线程，则延续回调应该被放入主线程处理
            if (SynchronizationContext.Current != task._synchronizationContext)
            {
                task._synchronizationContext.Post(task.RequestCancelInternal, task);
            }
            else
            {
                // 否则直接执行
                task.RequestCancelInternal(state);
            }
        }
        
        /// <summary>
        /// 取消回调封装
        /// </summary>
        /// <param name="state"></param>
        private void RequestCancelInternal(object state)
        {
            // 检查是否已完成，防止重复处理
            if (_isCompleted)
            {
                return;
            }

            // 标记任务完成
            _isCompleted = true;
            // 移除原生回调，避免内存泄漏
            _operation.completed -= RequestCompleted;
            // 标记取消异常，供后续抛出
            _exception = new OperationCanceledException(_cancellationToken);
            // 释放取消令牌注册器，取消监听
            _cancellationTokenRegistration.Dispose();
            // 获取所有要执行的延迟任务
            var continuations = _continuations.ToArray();
            _continuations.Clear();
            // 如果已设置延续回调，触发回调通知任务完成
            ExecuteContinuation(continuations);
        }
        
        /// <summary>
        /// 设置任务完成后的延续回调
        /// </summary>
        /// <param name="continuation">延续执行的委托</param>
        public void SetContinuation(Action continuation)
        {
            if(continuation == null)
                return;
            
            if (_isCompleted)
            {
                continuation.Invoke();
            }
            else
            {
                _continuations.Add(continuation);
            }
        }
        
        /// <summary>
        /// AsyncOperation完成回调
        /// </summary>
        /// <param name="operation">AsyncOperation对象</param>
        private void RequestCompleted(AsyncOperation operation)
        {
            // 防止重复调用（任务可能已被取消）
            if (_isCompleted)
            {
                return;
            }

            Action[] continuations;
            try
            {
                OnRequestCompleted();
            }
            catch(Exception e)
            {
                _exception = e;
            }
            finally
            {
                // 移除原生回调，避免内存泄漏
                _operation.completed -= RequestCompleted;
                // 释放取消令牌注册器，取消监听
                _cancellationTokenRegistration.Dispose();
                // 修改状态
                _isCompleted = true;
                // 获取所有要执行的延迟任务
                continuations = _continuations.ToArray();
                _continuations.Clear();
            }
            
            // 执行延续
            ExecuteContinuation(continuations);
        }

        /// <summary>
        /// AsyncOperation完成时触发，处理各自的结果
        /// </summary>
        protected virtual void OnRequestCompleted()
        {
            
        }

        /// <summary>
        /// 执行全部延续任务
        /// </summary>
        /// <param name="state"></param>
        private static void ExecuteContinuation(object state)
        {
            var continuations = (Action[])state;
            foreach (var continuation in continuations)
            {
                try
                {
                    continuation?.Invoke();
                }
                catch (Exception e)
                {
                    Logger.LogException(ELogTags.Task, e);
                }
            }
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void GetResult()
        {
            if(_exception != null)
            {
                throw _exception;
            }
        }

        /// <summary>
        /// 获取等待器
        /// </summary>
        /// <returns></returns>
        public AoTaskAwaiter GetAwaiter()
        {
            return new AoTaskAwaiter(this);
        }
        
        void IPoolData.ResetData()
        {
            OnResetData();
            _operation = null;
            _continuations.Clear();
            _exception = null;
            _isCompleted = false;
            _synchronizationContext = null;
            _cancellationTokenRegistration = default;
            _cancellationToken = CancellationToken.None;
        }

        /// <summary>
        /// 被回收到对象池时调用，执行清理
        /// </summary>
        protected virtual void OnResetData()
        {
            
        }
        
        /// <summary>
        /// 释放任务，回收到对象池
        /// </summary>
        [Obsolete("Lagacy design, use dispose now", true)]
        internal void Release()
        {
            _poolManager.PushData(this);
        }

        public void Dispose()
        {
            _poolManager.PushData(this);
        }
    }

    /// <summary>
    /// 有返回值自定义泛型任务类
    /// </summary>
    /// <typeparam name="TResult">返回值结果类型</typeparam>
    internal class AoTask<TResult> : AoTask
    {
        /// <summary>
        /// 结果返回值
        /// </summary>
        protected TResult result;
        
        /// <summary>
        /// 获取任务执行结果
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public new TResult GetResult()
        {
            return _exception != null ? throw _exception : result;
        }

        /// <summary>
        /// 任务的异步等待器，支持await
        /// </summary>
        /// <returns></returns>
        public new AoTaskAwaiter<TResult> GetAwaiter()
        {
            return new AoTaskAwaiter<TResult>(this);
        }
        
        protected override void OnResetData()
        {
            result = default;
        }
    }
}
