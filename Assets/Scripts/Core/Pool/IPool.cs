namespace Core.Pool
{
    /// <summary>
    /// 对象池对象接口
    /// </summary>
    internal interface IPool
    {
        /// <summary>
        /// 池子ID——对象名称
        /// </summary>
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
        
        /// <summary>
        /// 清理池子所有缓存
        /// </summary>
        void ClearAll();
        
        /// <summary>
        /// 修剪池子——清理对象到最小容量
        /// </summary>
        void Trim();
    }
    
    /// <summary>
    /// 对象池对象泛型接口
    /// </summary>
    internal interface IPool<T> : IPool
    {
        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        T Get();
        
        /// <summary>
        /// 放入
        /// </summary>
        /// <param name="obj"></param>
        void Push(T obj);
    }
}
