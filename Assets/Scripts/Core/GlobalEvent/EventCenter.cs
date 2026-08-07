using System;
using System.Collections.Generic;
using Core.Exceptions;
using Core.Global;
using Core.Log;
using Core.Mono;
using Core.Pool;

namespace Core.GlobalEvent
{
    /// <summary>
    /// 事件中心
    /// 职责：统一管理事件的订阅、取消订阅、触发、延迟触发，支持按类型过滤事件
    /// 特性：单例模式、每帧限制延迟事件触发数量，避免单帧事件过多导致性能问题
    /// </summary>
    public class EventCenter : IEventCenter
    {
        private IPoolManager _poolManager;
        // 存储事件类型与对应事件信息列表的映射表。Key：事件类型（TEvent），Value：该类型下所有订阅的事件信息
        private readonly Dictionary<Type, List<IEventInfo>> _typeToEventInfoMap = new();
        // 延迟触发的事件队列，用于异步/分帧处理事件
        private readonly Queue<IEvent> _delayEvents = new();
        // 每帧允许触发的最大延迟事件数量
        private readonly int _eventTriggerMaxNumPerFrame;
        // 当前帧已触发的延迟事件数量，用于控制单帧触发上限
        private byte _currentTriggeredEventCount;
        // 事件触发最大递归深度
        private const byte _eventTriggerMaxRecursionDepth = 15;
        // 事件列表触发快照
        private readonly List<IEventInfo> _eventInfoSnapshots = new();

        /// <summary>
        /// 私有构造函数（单例模式）
        /// 初始化：注册Update监听，用于每帧处理延迟事件队列
        /// </summary>
        private EventCenter(IMonoAdapter monoAdapter, IPoolManager poolManager)
        {
            EventSource.Init(poolManager);
            monoAdapter.AddUpdateListener(OnUpdate);
            _eventTriggerMaxNumPerFrame = GlobalSettings.Instance.eventModuleConfig.eventTriggerMaxNumPerFrame;
        }

        /// <summary>
        /// 同步触发指定类型的事件
        /// </summary>
        /// <typeparam name="TEvent">事件类型，需实现IEvent接口</typeparam>
        /// <param name="evt">事件实例，携带事件相关数据</param>
        public void TriggerEvent<TEvent>(TEvent evt) where TEvent : class ,IEvent
        {
            // 查找该事件类型下所有订阅的事件信息
            if (_typeToEventInfoMap.TryGetValue(typeof(TEvent), out var eventInfos))
            {
                _eventInfoSnapshots.Clear();
                _eventInfoSnapshots.AddRange(eventInfos);
                
                // 遍历触发所有匹配的事件回调
                foreach (var eventInfoSnapshot in _eventInfoSnapshots)
                {
                    try
                    {
                        if (eventInfoSnapshot.RecursionDepth > _eventTriggerMaxRecursionDepth)
                            throw new InvalidOperationException($"Recursion depth {_eventTriggerMaxRecursionDepth} is exceeded");
                        
                        var eventInfo = (EventInfo<TEvent>)eventInfoSnapshot;
                        eventInfo.Invoke(evt);
                    }
                    catch (Exception e)
                    {
                        Logger.LogException(ELogTags.System, ExceptionHelper.ThrowEventTriggerException(typeof(TEvent), e));
                    }
                }
                
                EventSource.Collect(evt);
            }
        }

        /// <summary>
        /// 分帧触发事件（加入队列，由Update分帧处理）
        /// </summary>
        /// <typeparam name="TEvent">事件类型，需实现IEvent接口</typeparam>
        /// <param name="evt">事件实例，携带事件相关数据</param>
        public void TriggerEventAsync<TEvent>(TEvent evt) where TEvent : class, IEvent
        {
            // 放入延迟队列
            _delayEvents.Enqueue(evt);
        }

        /// <summary>
        /// 订阅指定类型的事件
        /// </summary>
        /// <typeparam name="TEvent">事件类型，需实现IEvent接口</typeparam>
        /// <param name="callBack">事件触发时执行的回调方法</param>
        /// <param name="filter">事件过滤条件（可选）：返回true则触发回调，false则跳过</param>
        public void SubscribeEvent<TEvent>(Action<TEvent> callBack, Func<TEvent, bool> filter = null) where TEvent : class, IEvent
        {
            var eventType = typeof(TEvent);
            // 封装事件回调与过滤条件为事件信息对象
            var eventInfo = _poolManager.GetData<EventInfo<TEvent>>();
            eventInfo.CallBack = callBack;
            eventInfo.Filter = filter;
            // 事件类型已存在：追加到现有列表；不存在：新建列表并添加
            if (_typeToEventInfoMap.TryGetValue(eventType, out var baseEventInfos))
            {
                baseEventInfos.Add(eventInfo);
            }
            else
            {
                _typeToEventInfoMap.Add(eventType, new List<IEventInfo> { eventInfo });
            }
        }

        /// <summary>
        /// 取消订阅指定类型的事件（根据回调方法匹配）
        /// </summary>
        /// <typeparam name="TEvent">事件类型，需实现IEvent接口</typeparam>
        /// <param name="callBack">需要取消的事件回调方法</param>
        /// <param name="filter"></param>
        public void UnsubscribeEvent<TEvent>(Action<TEvent> callBack, Func<TEvent, bool> filter = null) where TEvent : class, IEvent
        {
            // 查找该事件类型下的所有订阅信息
            if (!_typeToEventInfoMap.TryGetValue(typeof(TEvent), out var eventInfos))
                return;
            
            // 倒序遍历：避免删除元素导致索引错乱
            for (var i = eventInfos.Count - 1; i >= 0; i--)
            {
                var eventInfo = (EventInfo<TEvent>)eventInfos[i];
                // 匹配到目标回调则移除，并终止遍历
                if (eventInfo?.CallBack != callBack || eventInfo?.Filter != filter)
                    continue;
                
                _poolManager.PushData(eventInfo);
                eventInfos.RemoveAt(i);
                break;
            }
        }

        /// <summary>
        /// 移除指定类型的所有事件订阅
        /// </summary>
        /// <typeparam name="TEvent">事件类型，需实现IEvent接口</typeparam>
        public void RemoveEventsFrom<TEvent>() where TEvent : class, IEvent
        {
            var eventType = typeof(TEvent);
            // 移除该事件类型的所有订阅信息
            if (_typeToEventInfoMap.TryGetValue(eventType, out var eventInfos))
            {
                foreach (var baseEventInfo in eventInfos)
                {
                    _poolManager.PushData((EventInfo<TEvent>)baseEventInfo);
                }
                _typeToEventInfoMap.Remove(eventType);
            }
        }

        /// <summary>
        /// 每帧更新回调（由MonoManager驱动）
        /// 职责：处理延迟事件队列，控制单帧触发数量上限
        /// </summary>
        private void OnUpdate()
        {
            // 队列无事件时直接返回
            while(_delayEvents.Count > 0)
            {
                // 达到单帧触发上限：重置计数并退出，剩余事件留到下一帧处理
                if (_currentTriggeredEventCount >= _eventTriggerMaxNumPerFrame)
                {
                    _currentTriggeredEventCount = 0;
                    return;
                }
                
                // 出队并执行延迟事件
                TriggerEvent(_delayEvents.Dequeue());
                // 累计当前帧触发数量
                ++_currentTriggeredEventCount;
            }
        }
    }
}