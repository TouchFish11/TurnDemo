using System;

namespace Core.Tasks
{
    /// <summary>
    /// 任务句柄
    /// </summary>
    public struct TaskHandle : IDisposable
    {
        // 句柄ID
        private readonly int _id;
        // 任务的引用计数
        private uint _refCount; 
        // 任务对象
        private FTask _task;
        
        /// <summary>
        /// 获取内部的任务对象，每次访问该属性会是的引用计数增加
        /// </summary>
        public FTask Task
        {
            get
            {
                ++_refCount;
#if UNITY_EDITOR && DEBUG_TEST
                Logger.Log($"[TaskHandle]: id({_id}) Task 引用数增加到: {_refCount}");
#endif
                return _task;
            }
        }
        
        /// <summary>
        /// 内部的任务对象是否有效
        /// </summary>
        public bool IsValid => _task != null;
        
        public TaskHandle(FTask fTask)
        {
            _id = TaskHandleHelper.GetGlobalId();
            _task = fTask;
            _refCount = 0;
        }
        
        /// <summary>
        /// 减少引用计数，销毁句柄，销毁要和访问次数配对
        /// </summary>
        public void Dispose()
        {
            if (_refCount > 0)
            {
                --_refCount;
            }

#if UNITY_EDITOR && DEBUG_TEST
            Logger.Log($"[TaskHandle]: id({_id}) Task 引用数释放到: {_refCount}");
#endif
            if (_refCount == 0)
            {
                _task?.Release();
                _task = null;
            }
        }
    }
    
    /// <summary>
    /// 泛型任务句柄
    /// </summary>
    public struct TaskHandle<T> : IDisposable
    {
        // 句柄ID
        private readonly int _id;
        // 任务的引用计数
        private uint _refCount; 
        // 任务对象
        private FTask<T> _task;

        /// <summary>
        /// 获取内部的任务对象，每次访问该属性会是的引用计数增加
        /// </summary>
        public FTask<T> Task
        {
            get
            {
                ++_refCount;
#if UNITY_EDITOR && DEBUG_TEST
                Logger.Log($"[TaskHandle]: id({_id}) Task 引用数增加到: {_refCount}");
#endif
                return _task;
            }
        }

        /// <summary>
        /// 内部的任务对象是否有效
        /// </summary>
        public bool IsValid => _task != null;
        
        public TaskHandle(FTask<T> fTask)
        {
            _id = TaskHandleHelper.GetGlobalId();
            _task = fTask;
            _refCount = 0;
        }
        
        /// <summary>
        /// 减少引用计数，销毁句柄，销毁要和访问次数配对
        /// </summary>
        public void Dispose()
        {
            if (_refCount > 0)
            {
                --_refCount;
            }

#if UNITY_EDITOR && DEBUG_TEST
            Logger.Log($"[TaskHandle]: id({_id}) Task 引用数释放到: {_refCount}");
#endif
            if (_refCount == 0)
            {
                _task?.Release();
                _task = null;
            }
        }
    }
}
