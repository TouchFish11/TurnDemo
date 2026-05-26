namespace Core.GlobalEvent
{
    public interface IEventFactory
    {
        /// <summary>
        /// 获取指定类型的事件实例
        /// </summary>
        /// <typeparam name="TEvent">事件类型，需实现 IEvent 接口且为引用类型</typeparam>
        /// <returns>
        /// 返回重置后的事件实例
        /// </returns>
        TEvent GetEvent<TEvent>() where TEvent : Event;
    }
}
