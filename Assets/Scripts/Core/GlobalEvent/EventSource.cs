using Core.Pool;

namespace Core.GlobalEvent
{
    /// <summary>
    /// 事件源类，负责事件对象的创建与复用管理
    /// </summary>
    public static class EventSource
    {
        private static IPoolManager _poolManager;

        internal static void Init(IPoolManager poolManager)
        {
            _poolManager = poolManager;
        }
        
        /// <summary>
        /// 获取事件对象
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        public static TEvent Get<TEvent>() where TEvent : Event
        {
            return _poolManager.GetData<TEvent>();
        }

        /// <summary>
        /// 回收事件对象
        /// </summary>
        /// <param name="evt"></param>
        /// <typeparam name="TEvent"></typeparam>
        internal static void Collect<TEvent>(TEvent evt) where TEvent : class, IEvent
        {
            _poolManager.PushData(evt);
        }
    }
}