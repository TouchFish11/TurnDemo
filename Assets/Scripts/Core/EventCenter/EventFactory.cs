using Core.Reflection;
using Core.Utility;

namespace Core.EventCenter
{
    /// <summary>
    /// 事件工厂类，负责事件对象的创建与复用管理
    /// 继承自通用工厂基类 Factory，限定泛型类型为 IEvent 接口实现类
    /// </summary>
    public class EventFactory : Factory<IEvent>, IEventFactory
    {
        /// <summary>
        /// 获取指定类型的事件实例
        /// </summary>
        /// <typeparam name="TEvent">事件类型，需实现 IEvent 接口且为引用类型</typeparam>
        /// <returns>
        /// 若类型映射表中存在该事件类型的实例，返回重置后的事件实例；
        /// 若不存在，返回该类型的默认值（null）
        /// </returns>
        public TEvent GetEvent<TEvent>() where TEvent : class, IEvent
        {
            // 尝试从类型-接口映射字典中获取已缓存的事件实例
            if (!typeToInterfaceMap.TryGetValue(typeof(TEvent).ToIdentifier(), out var value))
            {
                return null;
            }
            
            // 将获取到的实例转换为目标事件类型
            var evt = value as TEvent;
            // 重置事件实例的状态，确保复用前处于初始状态
            evt?.ResetEvent();
            // 返回重置后的事件实例
            return evt;
        }
    }
}