using System;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;

namespace Core.Tasks.Awaiter
{
    public readonly struct AssetBundleRequestsAwaiter<T> : ICriticalNotifyCompletion where T : Object
    {
        // 持有对应的AssetBundle请求任务实例
        private readonly AssetBundleRequestsTask<T> _task;

        /// <summary>
        /// 获取一个值，指示异步任务是否已完成
        /// </summary>
        public bool IsCompleted => _task.IsCompleted;

        /// <summary>
        /// 初始化AssetBundleRequestAwaiter实例
        /// </summary>
        /// <param name="task">对应的AssetBundle请求任务</param>
        public AssetBundleRequestsAwaiter(AssetBundleRequestsTask<T> task)
        {
            _task = task;
        }

        /// <summary>
        /// 注册异步操作完成时要执行的回调（普通版本）
        /// </summary>
        /// <param name="continuation">任务完成后要执行的委托</param>
        public void OnCompleted(Action continuation)
        {
            // 复用不安全版本的完成回调注册逻辑
            UnsafeOnCompleted(continuation);
        }
        
        /// <summary>
        /// 注册异步操作完成时要执行的回调
        /// </summary>
        /// <param name="continuation">任务完成后要执行的委托</param>
        public void UnsafeOnCompleted(Action continuation)
        {
            // 将完成回调设置到任务实例中
            _task.SetContinuation(continuation);
        }
        
        /// <summary>
        /// 获取异步任务的执行结果（已加载的资源）
        /// </summary>
        /// <returns>加载完成的T类型资源</returns>
        public void GetResult()
        {
            _task.GetResult();
        }
    }
}
