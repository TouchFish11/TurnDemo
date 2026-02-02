using System;

namespace Core.GlobalEvent
{
    /// <summary>
    /// 事件中心核心接口
    /// 定义事件的触发、延迟触发、订阅、取消订阅等核心行为规范
    /// </summary>
    public interface IEventCenter
    {
        /// <summary>
        /// 立即触发指定类型的事件
        /// </summary>
        /// <typeparam name="TEvent">事件类型，必须实现IEvent接口</typeparam>
        /// <param name="evt">待触发的事件实例</param>
        void TriggerEvent<TEvent>(TEvent evt) where TEvent : IEvent;

        /// <summary>
        /// 延迟触发指定类型的事件
        /// 可通过过滤器条件控制是否最终执行回调
        /// </summary>
        /// <typeparam name="TEvent">事件类型，必须实现IEvent接口</typeparam>
        /// <param name="callBack">事件触发时执行的回调方法</param>
        /// <param name="evt">待触发的事件实例</param>
        /// <param name="filter">可选的事件过滤器，返回true时才执行回调，默认null（不过滤）</param>
        void DelayTriggerEvent<TEvent>(Action<TEvent> callBack, TEvent evt, Func<TEvent, bool> filter = null) where TEvent : IEvent;

        /// <summary>
        /// 订阅指定类型的事件
        /// 当该类型事件触发时，符合过滤条件的回调会被执行
        /// </summary>
        /// <typeparam name="TEvent">事件类型，必须实现IEvent接口</typeparam>
        /// <param name="callBack">事件触发时执行的回调方法</param>
        /// <param name="filter">可选的事件过滤器，返回true时才执行回调，默认null（不过滤）</param>
        void SubscribeEvent<TEvent>(Action<TEvent> callBack, Func<TEvent, bool> filter = null) where TEvent : IEvent;

        /// <summary>
        /// 取消订阅指定类型事件的指定回调方法
        /// </summary>
        /// <typeparam name="TEvent">事件类型，必须实现IEvent接口</typeparam>
        /// <param name="callBack">需要取消订阅的回调方法</param>
        void UnsubscribeEvent<TEvent>(Action<TEvent> callBack) where TEvent : IEvent;

        /// <summary>
        /// 移除指定类型事件的所有订阅回调
        /// 清空该事件类型下的所有监听逻辑
        /// </summary>
        /// <typeparam name="TEvent">事件类型，必须实现IEvent接口</typeparam>
        void RemoveEventsFrom<TEvent>() where TEvent : IEvent;

        /// <summary>
        /// 清空事件中心中所有类型事件的所有订阅回调
        /// 重置事件中心至初始状态
        /// </summary>
        void Clear();
    }
}