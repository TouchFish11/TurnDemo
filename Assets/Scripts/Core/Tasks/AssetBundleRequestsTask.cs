using System;
using System.Collections.Generic;
using System.Threading;
using Core.Pool;
using Core.Tasks.Awaiter;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Tasks
{
    public class AssetBundleRequestsTask<T> : IPoolData where T : Object
    {
        // AssetBundle原生请求对象
        private AssetBundleRequest _abr;
        // 取消令牌，用于监听取消请求
        private CancellationToken _cancellationToken;
        // 取消令牌注册器，用于释放取消监听
        private CancellationTokenRegistration _cancellationTokenRegistration;
        
        // 任务完成后的延续回调
        private Action _continuation;
        // 加载成功后的资源结果
        private IList<T> _result;
        // 任务执行过程中抛出的异常
        private Exception _exception;
        // 任务是否完成（volatile保证多线程可见性）
        private volatile bool _isCompleted;
        
        /// <summary>
        /// 任务是否已完成（完成包括成功、失败、取消）
        /// </summary>
        public bool IsCompleted => _isCompleted;

        /// <summary>
        /// 初始化AssetBundle请求任务
        /// </summary>
        /// <param name="request">原生AssetBundle请求对象</param>
        /// <param name="assets"></param>
        /// <param name="token">取消令牌，默认空（不监听取消）</param>
        public void Init(AssetBundleRequest request, IList<T> assets, CancellationToken token = default)
        {
            _abr = request;
            _result = assets;
            // 注册原生请求完成的回调
            _abr.completed += OnRequestCompleted;
            _cancellationToken = token;
            
            // 如果取消令牌可取消，则注册取消回调
            if (_cancellationToken.CanBeCanceled)
            {
                _cancellationTokenRegistration = _cancellationToken.Register(state =>
                {
                    var task = (AssetBundleRequestsTask<T>)state;
                    // 检查是否已完成，防止重复处理（第一层检查，非线程安全）
                    if (task.IsCompleted)
                    {
                        return;
                    }

                    // 加锁保证并发安全，防止多线程同时处理取消和完成
                    lock (task)
                    {
                        // 双重检查，防止并发场景下的重复处理（第二层检查，线程安全）
                        if (task.IsCompleted)
                        {
                            return;
                        }
                        
                        // 尝试获取已加载的资源并执行卸载逻辑（如果资源已加载完成）
                        var ab = task._abr.asset;
                        if (ab != null)
                        {
                            // TODO：通知管理器卸载该资源
                            //ServiceLocator.Get<IAssetBundleManager>().UnloadBundleAsync()
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
        /// <param name="continuation">延续执行的委托</param>
        public void SetContinuation(Action continuation)
        {
            _continuation = continuation;
        }
        
        /// <summary>
        /// 获取任务执行结果
        /// </summary>
        /// <returns>加载成功的资源对象</returns>
        /// <exception cref="Exception">任务执行过程中抛出的异常（包括取消异常）</exception>
        public void GetResult()
        {
            // 如果有异常则抛出
            if (_exception != null)
            {
                throw _exception;
            }
        }

        /// <summary>
        /// 原生AssetBundle请求完成的回调处理
        /// </summary>
        /// <param name="operation">异步操作对象（AssetBundleRequest）</param>
        private void OnRequestCompleted(AsyncOperation operation)
        {
            try
            {
                // 仅当未触发取消请求时，才获取加载结果
                if (!_cancellationToken.IsCancellationRequested)
                {
                    foreach (var asset in _abr.allAssets)
                    {
                        _result.Add(asset as T);
                    }
                    
                    // 无论是否异常，都标记任务完成
                    _isCompleted = true;
                    // 执行延续回调，通知任务完成
                    _continuation?.Invoke();
                }
            }
            finally
            {
                // 移除原生回调，避免内存泄漏
                _abr.completed -= OnRequestCompleted;
                // 释放取消令牌注册器，取消监听
                _cancellationTokenRegistration.Dispose();
            }
        }
        
        /// <summary>
        /// 获取异步等待器，支持await语法
        /// </summary>
        /// <returns>AssetBundle请求等待器</returns>
        public AssetBundleRequestsAwaiter<T> GetAwaiter()
        {
            return new AssetBundleRequestsAwaiter<T>(this);
        }

        public void ResetData()
        {
            _abr = null;
            _continuation = null;
            _result = null;
            _exception = null;
            _isCompleted = false;
        }
    }
}
