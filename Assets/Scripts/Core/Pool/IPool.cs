namespace Core.Pool
{
    /// <summary>
    /// 对象池接口
    /// </summary>
    internal interface IPool
    {
        string PoolId { get; }
        
        /// <summary>
        /// 标记池是否惰性
        /// </summary>
        bool IsLazy { get; }
        
        /// <summary>
        /// 上次Get/Push的时间，越小则越早使用
        /// </summary>
        float LastUsedTime { get; }
        
        /// <summary>
        /// 使用对象数
        /// </summary>
        int ActiveCount { get; }
        
        /// <summary>
        /// 未使用的对象数
        /// </summary>
        int InactiveCount { get; }
        
        void ClearAll();
        void Trim();
    }
    
    /// <summary>
    /// 对象池泛型接口
    /// </summary>
    internal interface IPool<T> : IPool
    {
        T Get();
        
        void Push(T obj);
    }
}
