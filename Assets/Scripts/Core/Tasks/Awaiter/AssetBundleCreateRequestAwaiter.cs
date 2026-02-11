using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Core.Tasks.Awaiter
{
    /// <summary>
    /// 用于异步等待AssetBundleCreateRequest完成的自定义Awaiter结构体
    /// 实现ICriticalNotifyCompletion接口以支持高效的异步等待逻辑
    /// </summary>
    public readonly struct AssetBundleCreateRequestAwaiter : ICriticalNotifyCompletion
    {
        // 持有对应的AssetBundle创建任务实例
        private readonly AssetBundleCreateRequestTask _task;
        
        /// <summary>
        /// 获取一个值，指示异步任务是否已完成
        /// </summary>
        public bool IsCompleted => _task.IsCompleted;

        /// <summary>
        /// 初始化AssetBundleCreateRequestAwaiter实例
        /// </summary>
        /// <param name="task">对应的AssetBundle创建任务</param>
        public AssetBundleCreateRequestAwaiter(AssetBundleCreateRequestTask task)
        {
            _task = task;
        }

        /// <summary>
        /// 注册当异步操作完成时要调用的延续操作
        /// </summary>
        /// <param name="continuation">异步操作完成后执行的委托</param>
        public void OnCompleted(Action continuation)
        {
            // 复用不安全的完成回调实现
            UnsafeOnCompleted(continuation);
        }
        
        /// <summary>
        /// 注册延续操作，不捕获执行上下文（性能更优）
        /// 实现ICriticalNotifyCompletion接口的核心方法
        /// </summary>
        /// <param name="continuation">异步操作完成后执行的委托</param>
        public void UnsafeOnCompleted(Action continuation)
        {
            _task.SetContinuation(continuation);
        }
        
        /// <summary>
        /// 获取异步操作的结果
        /// </summary>
        /// <returns>加载完成的AssetBundle实例</returns>
        public AssetBundle GetResult()
        {
            return _task.GetResult();
        }
    }
}