using System;
using System.Runtime.CompilerServices;

namespace Core.Tasks.Awaiter
{
    /// <summary>
    /// AssetBundle卸载操作的等待器（Awaiter）
    /// 实现ICriticalNotifyCompletion接口，支持异步await语法，用于等待AssetBundle卸载操作完成
    /// </summary>
    public class AssetBundleUnloadOperationAwaiter : ICriticalNotifyCompletion
    {
        // 持有对应的AssetBundle卸载任务实例，作为异步操作的核心载体
        private readonly AssetBundleUnloadOperationTask _task;
        
        /// <summary>
        /// 指示异步卸载任务是否已完成（供awaiter框架判断是否需要挂起等待）
        /// </summary>
        public bool IsCompleted => _task.IsCompleted;

        /// <summary>
        /// 初始化AssetBundle卸载操作等待器实例
        /// </summary>
        /// <param name="task">对应的AssetBundle卸载任务实例，不可为空</param>
        public AssetBundleUnloadOperationAwaiter(AssetBundleUnloadOperationTask task)
        {
            // 注入核心任务实例，后续所有等待逻辑均基于此任务
            _task = task;
        }

        /// <summary>
        /// 注册异步操作完成时要执行的延续方法（捕获执行上下文）
        /// </summary>
        /// <param name="continuation">操作完成后要执行的委托</param>
        public void OnCompleted(Action continuation)
        {
            // 复用不安全的完成回调实现，统一延续逻辑处理
            UnsafeOnCompleted(continuation);
        }
        
        /// <summary>
        /// 注册异步操作完成时的延续方法（不捕获执行上下文，性能更优）
        /// </summary>
        /// <param name="continuation">操作完成后要执行的委托</param>
        public void UnsafeOnCompleted(Action continuation)
        {
            // 将延续委托注册到核心任务中，由任务在完成时触发执行
            _task.SetContinuation(continuation);
        }
        
        /// <summary>
        /// 获取异步卸载操作的执行结果（等待器核心方法，供await语法触发）
        /// 此处无返回值，仅用于确认操作完成并处理可能的异常
        /// </summary>
        public void GetResult()
        {
            // 调用任务的结果获取方法，触发异常传播（若任务执行出错）
            _task.GetResult();
        }
    }
}