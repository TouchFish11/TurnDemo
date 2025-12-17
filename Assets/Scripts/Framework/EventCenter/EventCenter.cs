using System.Collections.Generic;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 事件中心
    /// </summary>
    public class EventCenter : SingletonBase<EventCenter>
    {
        // 存储事件的字典
        private readonly Dictionary<E_EventType, BaseEventInfo> _eventDic = new Dictionary<E_EventType, BaseEventInfo>();
        // 事件队列
        private readonly Queue<E_EventType> _eventQueue = new Queue<E_EventType>();
        // 携带信息的事件队列
        private readonly Queue<(E_EventType, object)> _eventQueueT = new Queue<(E_EventType, object)>();
        // 每帧最大分发事件数
        private const byte EventTriggerMaxNumPerFrame = 20 / 2;
        // 当前触发事件数
        private byte _currentTriggeredEventCount;

        private EventCenter()
        {
            MonoManager.Instance.AddUpdateListener(OnUpdate);
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="eventType"></param>
        public void TriggerEvent(E_EventType eventType)
        {
            if (_eventDic.ContainsKey(eventType))
            {
                (_eventDic[eventType] as EventInfo)?.Invoke();
            }
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventType">事件类型</param>
        /// <param name="info">事件参数</param>
        public void TriggerEvent<T>(E_EventType eventType, T info)
        {
            if (_eventDic.ContainsKey(eventType))
            {
                (_eventDic[eventType] as EventInfo<T>)?.Invoke(info);
            }
        }

        /// <summary>
        /// 延迟触发事件
        /// </summary>
        /// <param name="eventType"></param>
        public void DelayTriggerEvent(E_EventType eventType)
        {
            _eventQueue.Enqueue(eventType);
        }

        /// <summary>
        /// 延迟触发事件
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="info"></param>
        public void DelayTriggerEvent(E_EventType eventType, object info)
        {
            _eventQueueT.Enqueue((eventType, info));
        }

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="callBack"></param>
        public void AddEventListener(E_EventType eventType, UnityAction callBack)
        {
            if (_eventDic.ContainsKey(eventType))
            {
                (_eventDic[eventType] as EventInfo).EventCallBack += callBack;
            }
            else
            {
                EventInfo eventInfo = new EventInfo(callBack);
                _eventDic.Add(eventType, eventInfo);
            }
        }

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventType"></param>
        /// <param name="callBack"></param>
        public void AddEventListener<T>(E_EventType eventType, UnityAction<T> callBack)
        {
            if (_eventDic.ContainsKey(eventType))
            {
                (_eventDic[eventType] as EventInfo<T>).EventCallBack += callBack;
            }
            else
            {
                EventInfo<T> eventInfo = new EventInfo<T>(callBack);
                _eventDic.Add(eventType, eventInfo);
            }
        }

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="callBack"></param>
        public void RemoveEventListener(E_EventType eventType, UnityAction callBack)
        {
            if (_eventDic.ContainsKey(eventType))
            {
                (_eventDic[eventType] as EventInfo).EventCallBack -= callBack;
            }
        }

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventType"></param>
        /// <param name="callBack"></param>
        public void RemoveEventListener<T>(E_EventType eventType, UnityAction<T> callBack)
        {
            if (_eventDic.ContainsKey(eventType))
            {
                (_eventDic[eventType] as EventInfo<T>).EventCallBack -= callBack;
            }
        }

        /// <summary>
        /// 移除指定类型所有事件
        /// </summary>
        /// <param name="eventType"></param>
        public void RemoveEventsFrom(E_EventType eventType)
        {
            if( _eventDic.ContainsKey(eventType))
            {
                _eventDic.Remove(eventType);
            }
        }

        /// <summary>
        /// 自定义帧更新
        /// </summary>
        private void OnUpdate()
        {
            while(_eventQueue.Count > 0 || _eventQueueT.Count > 0)
            {
                if (_currentTriggeredEventCount >= EventTriggerMaxNumPerFrame)
                {
                    _currentTriggeredEventCount = default;
                    return;
                }

                //分发无参数事件
                TriggerEvent(_eventQueue.Dequeue());
                //分发有参数的事件
                TriggerEvent(_eventQueueT.Dequeue().Item1, _eventQueueT.Dequeue().Item2);
                ++_currentTriggeredEventCount;
            }
        }

        /// <summary>
        /// 清空所有事件
        /// </summary>
        public void Clear()
        {
            _eventDic.Clear();
            _eventQueue.Clear();
            _eventQueueT.Clear();
        }
    }
}
