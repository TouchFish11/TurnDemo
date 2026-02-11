using System;
using System.Threading;
using Core.Pool;
using Core.Tasks.Awaiter;
using UnityEngine;

namespace Core.Tasks
{
    /// <summary>
    /// AssetBundle创建请求的异步任务封装类
    /// 用于管理AssetBundleCreateRequest的生命周期、取消逻辑和回调通知
    /// </summary>
    public class AssetBundleCreateRequestTask : IPoolData
    {
        // 原生的AssetBundle创建请求对象
        private AssetBundleCreateRequest _abcr;
        // 任务取消令牌，用于监听外部取消请求
        private CancellationToken _cancellationToken;
        // 取消令牌的注册器，用于注销取消回调，防止内存泄漏
        private CancellationTokenRegistration _cancellationTokenRegistration;
        
        // 任务完成后的延续回调
        private Action _continuation;
        // 任务执行结果：成功时存储加载的AssetBundle
        private AssetBundle _result;
        // 任务执行过程中抛出的异常（取消/加载失败）
        private Exception _exception;
        // 标记任务是否已完成（成功/失败/取消），volatile保证多线程可见性
        private volatile bool _isCompleted;

        /// <summary>
        /// 获取任务是否已完成（成功/失败/取消）
        /// </summary>
        public bool IsCompleted => _isCompleted;

        /// <summary>
        /// 初始化AssetBundle创建请求任务
        /// </summary>
        /// <param name="request">原生的AssetBundle创建请求</param>
        /// <param name="token">任务取消令牌，默认空（不支持取消）</param>
        public void Init(AssetBundleCreateRequest request, CancellationToken token = default)
        {
            _abcr = request;
            // 注册原生请求的完成回调
            _abcr.completed += OnRequestCompleted;
            _cancellationToken = token;
            
            // 如果取消令牌可用，注册取消回调
            if (_cancellationToken.CanBeCanceled)
            {
                _cancellationTokenRegistration = _cancellationToken.Register(state =>
                {
                    var task = (AssetBundleCreateRequestTask)state;
                    // 检查是否已完成，防止重复处理（取消回调可能在任务完成后触发）
                    if (task.IsCompleted)
                    {
                        return;
                    }

                    // 加锁保证线程安全，防止并发取消和完成回调冲突
                    lock (task)
                    {
                        // 双重检查锁定（DCL），再次确认任务未完成
                        if (task.IsCompleted)
                        {
                            return;
                        }
                        
                        // 尝试获取已加载的AssetBundle并卸载，避免资源泄漏
                        var ab = task._abcr.assetBundle;
                        if (ab != null)
                        {
                            // TODO：通知管理器卸载该AB包
                            // ServiceLocator.Get<IAssetBundleManager>().UnloadBundleAsync()
                        }
                    
                        // 标记任务为取消异常
                        _exception = new OperationCanceledException(token);
                        // 标记任务完成
                        _isCompleted = true;
                    }
                    
                    // 如果已设置延续回调，触发回调通知任务完成
                    task._continuation?.Invoke();
                    Debug.Log($"任务已取消");
                }, this);
            }
            else
            {
                // 取消令牌不可用，初始化空注册器
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
        /// 获取任务执行结果
        /// </summary>
        /// <returns>成功时返回加载的AssetBundle</returns>
        /// <exception cref="Exception">任务取消/失败时抛出对应的异常</exception>
        public AssetBundle GetResult()
        {
            // 如果有异常（取消/加载失败），抛出异常；否则返回结果
            return _exception != null ? throw _exception : _result;
        }

        /// <summary>
        /// 原生AssetBundle创建请求完成的回调方法
        /// </summary>
        /// <param name="operation">异步操作对象（实际为AssetBundleCreateRequest）</param>
        private void OnRequestCompleted(AsyncOperation operation)
        {
            // 防止重复调用（任务可能已被取消）
            if (_isCompleted)
            {
                return;
            }
            
            try
            {
                // 如果未触发取消请求，获取加载完成的AssetBundle
                if (!_cancellationToken.IsCancellationRequested)
                {
                    _result = _abcr.assetBundle;
                }
            }
            finally
            {
                // 无论成功/失败，标记任务完成
                _isCompleted = true;
                // 注销原生请求的完成回调，防止内存泄漏
                _abcr.completed -= OnRequestCompleted;
                // 注销取消令牌的注册器，防止内存泄漏
                _cancellationTokenRegistration.Dispose();
                // 触发延续回调，通知任务完成
                _continuation?.Invoke();
            }
        }
        
        /// <summary>
        /// 获取任务的异步等待器，支持await语法
        /// </summary>
        /// <returns>AssetBundle创建请求的等待器</returns>
        public AssetBundleCreateRequestAwaiter GetAwaiter()
        {
            return new AssetBundleCreateRequestAwaiter(this);
        }

        public void ResetData()
        {
            _abcr = null;
            _continuation = null;
            _result = null;
            _exception = null;
            _isCompleted = false;
        }
    }
}