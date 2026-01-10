using System;

namespace Framework
{
    /// <summary>
    /// 事件中心接口
    /// </summary>
    public interface IEventCenter
    {
        /// <summary>
        /// 触发事件
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="evt">事件类型</param>
        void TriggerEvent<TEvent>(TEvent evt) where TEvent : IEvent;

        /// <summary>
        /// 延迟触发事件
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="callBack"></param>
        /// <param name="evt"></param>
        /// <param name="filter"></param>
        void DelayTriggerEvent<TEvent>(Action<TEvent> callBack, TEvent evt, Func<TEvent, bool> filter = null) where TEvent : IEvent;

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="callBack"></param>
        /// <param name="filter"></param>
        void SubscribeEvent<TEvent>(Action<TEvent> callBack, Func<TEvent, bool> filter = null) where TEvent : IEvent;

        /// <summary>
        /// 移除事件订阅
        /// </summary>
        /// <param name="callBack"></param>
        void UnsubscribeEvent<TEvent>(Action<TEvent> callBack) where TEvent : IEvent;

        /// <summary>
        /// 移除指定类型所有事件订阅
        /// </summary>
        void RemoveEventsFrom<TEvent>() where TEvent : IEvent;

        /// <summary>
        /// 清空所有事件
        /// </summary>
        void Clear();
    }
}
