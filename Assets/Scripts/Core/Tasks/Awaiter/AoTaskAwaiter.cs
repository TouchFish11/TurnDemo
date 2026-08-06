using System;
using System.Runtime.CompilerServices;

namespace Core.Tasks.Awaiter
{
    /// <summary>
    /// 自定义Task无返回值等待器
    /// </summary>
    internal readonly struct AoTaskAwaiter : ICriticalNotifyCompletion
    {
        private readonly AoTask aoTask;

        public bool IsCompleted => aoTask.IsCompleted;
        
        public AoTaskAwaiter(AoTask aoTask)
        {
            this.aoTask = aoTask;
        }
        
        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            aoTask.SetContinuation(continuation);
        }

        public void GetResult()
        {
            aoTask.GetResult();
        }
    }
    
    /// <summary>
    /// 自定义Task有返回值等待器
    /// </summary>
    /// <typeparam name="TResult">返回类型</typeparam>
    internal readonly struct AoTaskAwaiter<TResult> : ICriticalNotifyCompletion
    {
        private readonly AoTask<TResult> aoTask;

        public bool IsCompleted => aoTask.IsCompleted;
        
        public AoTaskAwaiter(AoTask<TResult> aoTask)
        {
            this.aoTask = aoTask;
        }
        
        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            aoTask.SetContinuation(continuation);
        }

        public TResult GetResult()
        {
            return aoTask.GetResult();
        }
    }
}
