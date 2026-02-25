using System;
using System.Threading;
using Core.Pool;
using Core.Tasks.Awaiter;
using UnityEngine.Networking;
using AsyncOperation = UnityEngine.AsyncOperation;

namespace Core.Tasks
{
    /// <summary>
    /// UnityWebRequest异步操作任务
    /// </summary>
    public class UnityWebRequestAsyncOperationTask : IPoolData
    {
        // nityWebRequest异步操作
        private UnityWebRequestAsyncOperation _webRequestAsyncOperation;
        // 任务完成后需要执行的延续回调方法
        private Action _continuation;
        // 任务执行过程中捕获的异常信息
        private Exception _exception;
        // 标记任务是否已完成（成功/失败），volatile保证多线程下的内存可见性
        private volatile bool _isCompleted;
        // 取消令牌，用于监听取消请求
        private CancellationToken _cancellationToken;
        // 取消令牌注册器，用于释放取消监听
        private CancellationTokenRegistration _cancellationTokenRegistration;

        /// <summary>
        /// 获取任务是否已完成（成功/失败）
        /// </summary>
        public bool IsCompleted => _isCompleted;

        /// <summary>
        /// 初始化AssetBundle卸载任务
        /// </summary>
        /// <param name="requestAsyncOperation"></param>
        /// <param name="token"></param>
        public void Init(UnityWebRequestAsyncOperation requestAsyncOperation, CancellationToken token)
        {
            _cancellationToken = token;
            _webRequestAsyncOperation = requestAsyncOperation;
            // 注册原生异步操作的完成回调，监听操作结束事件
            _webRequestAsyncOperation.completed += OnRequestCompleted;
            
            // 如果取消令牌可取消，则注册取消回调
            if (_cancellationToken.CanBeCanceled)
            {
                _cancellationTokenRegistration = _cancellationToken.Register(state =>
                {
                    var task = (UnityWebRequestAsyncOperationTask)state;
                    // 检查是否已完成，防止重复处理
                    if (task.IsCompleted)
                    {
                        return;
                    }

                    // 加锁保证并发安全，防止多线程同时处理取消和完成
                    lock (task)
                    {
                        // 双重检查，防止并发场景下的重复处理
                        if (task.IsCompleted)
                        {
                            return;
                        }
                        
                        // 标记取消异常，供后续抛出
                        _exception = new OperationCanceledException(token);
                        // 标记任务完成
                        _isCompleted = true;
                    }
                    
                    // 如果已设置延续回调，执行回调通知任务完成
                    task._continuation?.Invoke();
                }, this);
            }
            else
            {
                // 不可取消的令牌，赋值默认注册器（空）
                _cancellationTokenRegistration = default;
            }
        }
        
        /// <summary>
        /// 设置任务完成后的延续回调
        /// </summary>
        /// <param name="continuation">任务完成后执行的回调方法</param>
        public void SetContinuation(Action continuation)
        {
            _continuation = continuation;
        }
        
        /// <summary>
        /// 获取任务执行结果，若有异常则抛出
        /// </summary>
        /// <exception cref="Exception">任务执行过程中捕获的异常</exception>
        public void GetResult()
        {
            // 如果存在异常，抛出异常给上层处理
            if (_exception != null)
            {
                throw _exception;
            }
        }
        
        
        private void OnRequestCompleted(AsyncOperation asyncOperation)
        {
            try
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                // 标记任务为已完成状态
                _isCompleted = true;
                // 触发延续回调
                _continuation?.Invoke();
            }
            catch (Exception exception)
            {
                // 捕获回调执行过程中的异常，暂存供后续GetResult时抛出
                _exception = exception;
            }
            finally
            {
                // 注销完成回调，避免重复回调和内存泄漏
                _webRequestAsyncOperation.completed -= OnRequestCompleted;
                _cancellationTokenRegistration.Dispose();
            }
        }
        
        /// <summary>
        /// 获取任务的异步等待器
        /// </summary>
        /// <returns>AssetBundle卸载操作的等待器对象</returns>
        public UnityWebRequestAsyncOperationAwaiter GetAwaiter()
        {
            return new UnityWebRequestAsyncOperationAwaiter(this);
        }
        
        /// <summary>
        /// 重置对象数据，供对象池复用
        /// </summary>
        public void ResetData()
        {
            // 清空所有成员变量，恢复初始状态
            _webRequestAsyncOperation = null;
            _continuation = null;
            _exception = null;
            _isCompleted = false;
        }
    }
}
