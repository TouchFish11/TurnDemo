using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 事件中心
    /// </summary>
    public class EventCenter : SingletonBase<EventCenter>, IEventCenter
    {
        // 存储事件的字典
        private readonly Dictionary<Type, List<BaseEventInfo>> _typeToEventInfoMap = new Dictionary<Type, List<BaseEventInfo>>();
        // 事件队列
        private readonly Queue<DelayEventInfo> _delayEventQueue = new Queue<DelayEventInfo>();
        // 当前触发事件数
        private byte _currentTriggeredEventCount;

        /// <summary>
        /// 每帧最大分发事件数
        /// </summary>
        private const byte EventTriggerMaxNumPerFrame = 10;

        private EventCenter()
        {
            MonoManager.Instance.AddUpdateListener(OnUpdate);
        }

        public void TriggerEvent<TEvent>(TEvent evt) where TEvent : IEvent
        {
            if (_typeToEventInfoMap.TryGetValue(typeof(TEvent), out var eventInfos))
            {
                foreach (var eventInfo in eventInfos)
                {
                    (eventInfo as EventInfo<TEvent>)?.Invoke(evt);
                }
            }
        }

        public void DelayTriggerEvent<TEvent>(Action<TEvent> callBack, TEvent evt, Func<TEvent, bool> filter = null) where TEvent : IEvent
        {
            _delayEventQueue.Enqueue(new DelayEventInfo() 
            { 
                TriggerCallback = () => callBack?.Invoke(evt), 
                Filter = () => filter.Invoke(evt)
            });
        }

        public void SubscribeEvent<TEvent>(Action<TEvent> callBack, Func<TEvent, bool> filter = null) where TEvent : IEvent
        {
            Type eventType = typeof(TEvent);
            EventInfo<TEvent> eventInfo = new EventInfo<TEvent>(callBack, filter);
            if (_typeToEventInfoMap.ContainsKey(eventType))
            {
                _typeToEventInfoMap[eventType].Add(eventInfo);
            }
            else
            {
                _typeToEventInfoMap.Add(eventType, new List<BaseEventInfo>() { eventInfo });
            }
        }

        public void UnsubscribeEvent<TEvent>(Action<TEvent> callBack) where TEvent : IEvent
        {
            if (_typeToEventInfoMap.TryGetValue(typeof(TEvent), out var eventInfos))
            {
                for (int i = eventInfos.Count - 1; i >= 0; i--)
                {
                    if ((eventInfos[i] as EventInfo<TEvent>).CallBack == callBack)
                    {
                        eventInfos.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void RemoveEventsFrom<TEvent>() where TEvent : IEvent
        {
            Type eventType = typeof(TEvent);
            if (_typeToEventInfoMap.ContainsKey(eventType))
            {
                _typeToEventInfoMap.Remove(eventType);
            }
        }

        /// <summary>
        /// 自定义帧更新
        /// </summary>
        private void OnUpdate()
        {
            while(_delayEventQueue.Count > 0)
            {
                if (_currentTriggeredEventCount >= EventTriggerMaxNumPerFrame)
                {
                    _currentTriggeredEventCount = default;
                    return;
                }

                _delayEventQueue.Dequeue().Invoke();
                ++_currentTriggeredEventCount;
            }
        }

        public void Clear()
        {
            _typeToEventInfoMap.Clear();
            _delayEventQueue.Clear();
        }
    }
}
