using Core.Reflection;

namespace Core.GlobalEvent
{
    public interface IEventFactory : IFactory
    {
        /// <summary>
        /// 获取指定类型的事件实例
        /// </summary>
        /// <typeparam name="TEvent">事件类型，需实现 IEvent 接口且为引用类型</typeparam>
        /// <returns>
        /// 若类型映射表中存在该事件类型的实例，返回重置后的事件实例；
        /// 若不存在，返回该类型的默认值（null）
        /// </returns>
        TEvent GetEvent<TEvent>() where TEvent : class, IEvent;
    }
}
