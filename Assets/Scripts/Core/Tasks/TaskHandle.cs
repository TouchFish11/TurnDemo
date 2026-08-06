using System;

namespace Core.Tasks
{
    /// <summary>
    /// 任务句柄
    /// </summary>
    [Obsolete("Redundancy of encapsulation", true)]
    internal struct TaskHandle : IDisposable
    {
#if UNITY_EDITOR
        // 句柄ID，调试用
        private readonly int _id;
#endif
        // 任务的引用计数
        private uint _refCount; 
        // 任务对象
        private AoTask _task;
        
        /// <summary>
        /// 获取内部的任务对象，每次访问该属性会是的引用计数增加
        /// </summary>
        public AoTask Task
        {
            get
            {
                ++_refCount;
                return _task;
            }
        }
        
        internal TaskHandle(AoTask aoTask)
        {
#if UNITY_EDITOR
            _id = TaskHandleHelper.GetGlobalId();   
#endif
            _task = aoTask;
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
            
            if (_refCount == 0)
            {
                //_task?.Release();
                _task = null;
            }
        }
    }
    
    /// <summary>
    /// 泛型任务句柄
    /// </summary>
    internal struct TaskHandle<T> : IDisposable
    {
#if UNITY_EDITOR
        // 句柄ID，调试用
        private readonly int _id;
#endif
        // 任务的引用计数
        private uint _refCount; 
        // 任务对象
        private AoTask<T> _task;

        /// <summary>
        /// 获取内部的任务对象，每次访问该属性会是的引用计数增加
        /// </summary>
        public AoTask<T> Task
        {
            get
            {
                ++_refCount;
                return _task;
            }
        }
        
        public TaskHandle(AoTask<T> aoTask)
        {
#if UNITY_EDITOR
            _id = TaskHandleHelper.GetGlobalId();   
#endif
            _task = aoTask;
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

            if (_refCount == 0)
            {
                //_task?.Release();
                _task = null;
            }
        }
    }
}
