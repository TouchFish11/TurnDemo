namespace Core.Systems.Memorys
{
    public interface IMemoryMonitor
    {
        /// <summary>
        /// 注册内存监听器
        /// </summary>
        /// <param name="listener"></param>
        void Register(IMemoryListener listener);
        
        /// <summary>
        /// 注销内存监听器
        /// </summary>
        /// <param name="listener"></param>
        void Unregister(IMemoryListener listener);
    }
}
