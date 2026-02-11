using System;
using Core.Pool;
using Core.Tasks.Awaiter;
using UnityEngine;

namespace Core.Tasks
{
    /// <summary>
    /// AssetBundle卸载操作的任务封装类
    /// 实现IPoolData接口支持对象池复用
    /// </summary>
    public class AssetBundleUnloadOperationTask : IPoolData
    {
        // 原生的AssetBundle卸载异步操作对象
        private AssetBundleUnloadOperation _abuo;
        // 任务完成后需要执行的延续回调方法
        private Action _continuation;
        // 任务执行过程中捕获的异常信息
        private Exception _exception;
        // 标记任务是否已完成（成功/失败），volatile保证多线程下的内存可见性
        private volatile bool _isCompleted;

        /// <summary>
        /// 获取任务是否已完成（成功/失败）
        /// </summary>
        public bool IsCompleted => _isCompleted;

        /// <summary>
        /// 初始化AssetBundle卸载任务
        /// </summary>
        /// <param name="unloadOperation">原生的AssetBundle卸载异步操作对象</param>
        public void Init(AssetBundleUnloadOperation unloadOperation)
        {
            _abuo = unloadOperation;
            // 注册原生异步操作的完成回调，监听操作结束事件
            _abuo.completed += OnRequestCompleted;
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

        /// <summary>
        /// 原生AssetBundle卸载操作完成的回调方法
        /// </summary>
        /// <param name="operation">完成的异步操作对象</param>
        private void OnRequestCompleted(AsyncOperation operation)
        {
            try
            {
                // 标记任务为已完成状态
                _isCompleted = true;
                // 注销完成回调，避免重复回调和内存泄漏
                _abuo.completed -= OnRequestCompleted;
                // 触发延续回调，通知上层任务已完成
                _continuation?.Invoke();
            }
            catch (Exception exception)
            {
                // 捕获回调执行过程中的异常，暂存供后续GetResult时抛出
                _exception = exception;
            }
        }
        
        /// <summary>
        /// 获取任务的异步等待器
        /// </summary>
        /// <returns>AssetBundle卸载操作的等待器对象</returns>
        public AssetBundleUnloadOperationAwaiter GetAwaiter()
        {
            return new AssetBundleUnloadOperationAwaiter(this);
        }

        /// <summary>
        /// 重置对象数据，供对象池复用
        /// 实现IPoolData接口的重置方法
        /// </summary>
        public void ResetData()
        {
            // 清空所有成员变量，恢复初始状态
            _abuo = null;
            _continuation = null;
            _exception = null;
            _isCompleted = false;
        }
    }
}