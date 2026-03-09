using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Mono;
using Core.Service;
using Core.Singleton;

namespace Core.GlobalEvent
{
    /// <summary>
    /// 事件中心
    /// 职责：统一管理事件的订阅、取消订阅、触发、延迟触发，支持按类型过滤事件
    /// 特性：单例模式、每帧限制延迟事件触发数量，避免单帧事件过多导致性能问题
    /// </summary>
    public class EventCenter : SingletonBase<EventCenter>, IEventCenter
    {
        public override int Priority => 0;

        // 存储事件类型与对应事件信息列表的映射表
        // Key：事件类型（TEvent），Value：该类型下所有订阅的事件信息
        private readonly Dictionary<Type, List<BaseEventInfo>> _typeToEventInfoMap = new Dictionary<Type, List<BaseEventInfo>>();
        // 延迟触发的事件队列，用于异步/分帧处理事件
        private readonly Queue<DelayEventInfo> _delayEventQueue = new Queue<DelayEventInfo>();
        // 当前帧已触发的延迟事件数量，用于控制单帧触发上限
        private byte _currentTriggeredEventCount;

        /// <summary>
        /// 每帧允许触发的最大延迟事件数量
        /// 限制阈值，防止单帧处理过多事件导致帧率下降
        /// </summary>
        private const byte EventTriggerMaxNumPerFrame = 10;

        /// <summary>
        /// 私有构造函数（单例模式）
        /// 初始化：注册Update监听，用于每帧处理延迟事件队列
        /// </summary>
        private EventCenter(){}

        public override Task InitAsync()
        {
            ServiceLocator.Get<IMonoAdapter>().AddUpdateListener(OnUpdate);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 同步触发指定类型的事件
        /// </summary>
        /// <typeparam name="TEvent">事件类型，需实现IEvent接口</typeparam>
        /// <param name="evt">事件实例，携带事件相关数据</param>
        public void TriggerEvent<TEvent>(TEvent evt) where TEvent : IEvent
        {
            // 查找该事件类型下所有订阅的事件信息
            if (_typeToEventInfoMap.TryGetValue(typeof(TEvent), out var eventInfos))
            {
                // 遍历触发所有匹配的事件回调
                for (var i = eventInfos.Count - 1; i >= 0; i--)
                {
                    (eventInfos[i] as EventInfo<TEvent>)?.Invoke(evt);
                }
            }
        }

        /// <summary>
        /// 延迟触发事件（加入队列，由Update分帧处理）
        /// </summary>
        /// <typeparam name="TEvent">事件类型，需实现IEvent接口</typeparam>
        /// <param name="callBack">事件触发时执行的回调方法</param>
        /// <param name="evt">事件实例，携带事件相关数据</param>
        /// <param name="filter">事件过滤条件（可选）：返回true则触发，false则跳过</param>
        public void DelayTriggerEvent<TEvent>(Action<TEvent> callBack, TEvent evt, Func<TEvent, bool> filter = null) where TEvent : IEvent
        {
            // 将延迟事件包装为队列元素，加入延迟队列
            _delayEventQueue.Enqueue(new DelayEventInfo
            { 
                TriggerCallback = () => callBack?.Invoke(evt), 
                Filter = () => filter?.Invoke(evt) ?? true // 无过滤条件时默认触发
            });
        }

        /// <summary>
        /// 订阅指定类型的事件
        /// </summary>
        /// <typeparam name="TEvent">事件类型，需实现IEvent接口</typeparam>
        /// <param name="callBack">事件触发时执行的回调方法</param>
        /// <param name="filter">事件过滤条件（可选）：返回true则触发回调，false则跳过</param>
        public void SubscribeEvent<TEvent>(Action<TEvent> callBack, Func<TEvent, bool> filter = null) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            // 封装事件回调与过滤条件为事件信息对象
            var eventInfo = new EventInfo<TEvent>(callBack, filter);
            
            // 事件类型已存在：追加到现有列表；不存在：新建列表并添加
            if (_typeToEventInfoMap.TryGetValue(eventType, out var baseEventInfos))
            {
                baseEventInfos.Add(eventInfo);
            }
            else
            {
                _typeToEventInfoMap.Add(eventType, new List<BaseEventInfo> { eventInfo });
            }
        }

        /// <summary>
        /// 取消订阅指定类型的事件（根据回调方法匹配）
        /// </summary>
        /// <typeparam name="TEvent">事件类型，需实现IEvent接口</typeparam>
        /// <param name="callBack">需要取消的事件回调方法</param>
        public void UnsubscribeEvent<TEvent>(Action<TEvent> callBack) where TEvent : IEvent
        {
            // 查找该事件类型下的所有订阅信息
            if (!_typeToEventInfoMap.TryGetValue(typeof(TEvent), out var eventInfos))
            {
                return;
            }
            
            // 倒序遍历：避免删除元素导致索引错乱
            for (var i = eventInfos.Count - 1; i >= 0; i--)
            {
                // 匹配到目标回调则移除，并终止遍历
                if ((eventInfos[i] as EventInfo<TEvent>)?.CallBack != callBack)
                {
                    continue;
                }
                
                eventInfos.RemoveAt(i);
                break;
            }
        }

        /// <summary>
        /// 移除指定类型的所有事件订阅
        /// </summary>
        /// <typeparam name="TEvent">事件类型，需实现IEvent接口</typeparam>
        public void RemoveEventsFrom<TEvent>() where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            // 移除该事件类型的所有订阅信息
            _typeToEventInfoMap.Remove(eventType);
        }

        /// <summary>
        /// 每帧更新回调（由MonoManager驱动）
        /// 职责：处理延迟事件队列，控制单帧触发数量上限
        /// </summary>
        private void OnUpdate()
        {
            // 队列无事件时直接返回
            while(_delayEventQueue.Count > 0)
            {
                // 达到单帧触发上限：重置计数并退出，剩余事件留到下一帧处理
                if (_currentTriggeredEventCount >= EventTriggerMaxNumPerFrame)
                {
                    _currentTriggeredEventCount = 0;
                    return;
                }

                // 出队并执行延迟事件
                _delayEventQueue.Dequeue().Invoke();
                // 累计当前帧触发数量
                ++_currentTriggeredEventCount;
            }
        }

        /// <summary>
        /// 清空所有事件数据（订阅列表+延迟队列）
        /// 适用场景：场景切换、游戏重启等需要重置事件中心的情况
        /// </summary>
        public void Clear()
        {
            _typeToEventInfoMap.Clear();
            _delayEventQueue.Clear();
        }
    }
}