using System;

namespace Framework
{
    /// <summary>
    /// 事件工厂
    /// </summary>
    public class EventFactory : Factory<IEvent>
    {
        /// <summary>
        /// 获取事件
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        public TEvent GetEvent<TEvent>() where TEvent : class, IEvent
        {
            if (typeToITypeMap.TryGetValue(typeof(TEvent), out var value))
            {
                TEvent evt = value as TEvent;
                evt.ResetEvent();
                return evt;
            }

            return default;
        }
    }
}
