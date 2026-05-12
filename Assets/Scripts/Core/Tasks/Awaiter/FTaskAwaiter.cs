using System;
using System.Runtime.CompilerServices;

namespace Core.Tasks.Awaiter
{
    /// <summary>
    /// 自定义Task无返回值等待器
    /// </summary>
    public readonly struct FTaskAwaiter : ICriticalNotifyCompletion
    {
        private readonly FTask _fTask;

        public bool IsCompleted => _fTask.IsCompleted;
        
        public FTaskAwaiter(FTask fTask)
        {
            _fTask = fTask;
        }
        
        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            _fTask.SetContinuation(continuation);
        }

        public void GetResult()
        {
            _fTask.GetResult();
        }
    }
    
    /// <summary>
    /// 自定义Task有返回值等待器
    /// </summary>
    /// <typeparam name="TResult">返回类型</typeparam>
    public readonly struct FTaskAwaiter<TResult> : ICriticalNotifyCompletion
    {
        private readonly FTask<TResult> _fTask;

        public bool IsCompleted => _fTask.IsCompleted;
        
        public FTaskAwaiter(FTask<TResult> fTask)
        {
            _fTask = fTask;
        }
        
        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            _fTask.SetContinuation(continuation);
        }

        public TResult GetResult()
        {
            return _fTask.GetResult();
        }
    }
}
