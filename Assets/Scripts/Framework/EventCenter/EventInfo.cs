using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 事件信息类
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public class EventInfo<TEvent> : BaseEventInfo where TEvent : IEvent
    {
        public Action<TEvent> CallBack { get; }
        public Func<TEvent, bool> Filter { get; }

        public EventInfo(Action<TEvent> callBack, Func<TEvent, bool> filter)
        {
            this.CallBack = callBack;
            this.Filter = filter;
        }

        /// <summary>
        /// 调用
        /// </summary>
        /// <param name="info">传递信息</param>
        public void Invoke(TEvent info)
        {
            if (Filter.Invoke(info))
            {
                CallBack?.Invoke(info);
            }
        }
    }
}
