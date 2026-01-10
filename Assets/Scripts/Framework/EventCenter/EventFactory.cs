using System;

namespace Framework
{
    /// <summary>
    /// 事件工厂
    /// </summary>
    public class EventFactory : Factory<IEvent, Attribute>
    {
        /// <summary>
        /// 获取事件
        /// 应使用GetEvent方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override T GetValue<T>()
        {
            return base.GetValue<T>();
        }

        /// <summary>
        /// 获取事件
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        public TEvent GetEvent<TEvent>() where TEvent : class, IEvent
        {
            if (typeToIStatusMap.TryGetValue(typeof(TEvent), out var value))
            {
                TEvent evt = value as TEvent;
                evt.ResetEvent();
                return evt;
            }

            return default;
        }
    }
}
