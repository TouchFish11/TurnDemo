using System;
using System.Collections.Generic;

namespace HotUpdate.Battle.Event
{
    public abstract class BaseBattleEventInfo
    {

    }

    public class BattleEventInfo<TEvent> : BaseBattleEventInfo where TEvent : BattleEvent
    {
        public event Action<TEvent> OnBattleEvent;

        public void Invoke(TEvent eventInfo)
        {
            OnBattleEvent?.Invoke(eventInfo);
        }
    }
    
    /// <summary>
    /// 战斗事件总线
    /// 局部事件总线，负责战斗流程中各模块间的事件通信
    /// </summary>
    public class BattleEventBus : IBattleEventBus
    {
        // 存储“事件类型→战斗事件信息”的映射（订阅者是接收事件的回调方法）
        private readonly Dictionary<Type, BaseBattleEventInfo> _typeToEventInfoMap = new();

        public void AddListener<TEvent>(Action<TEvent> callback) where TEvent : BattleEvent
        {
            var eventType = typeof(TEvent);
            if (!_typeToEventInfoMap.TryGetValue(eventType, out var eventInfo))
            {
                eventInfo = new BattleEventInfo<TEvent>();
                ((BattleEventInfo<TEvent>)eventInfo).OnBattleEvent += callback;
                _typeToEventInfoMap.Add(eventType, eventInfo);
            }
            else
            {
                ((BattleEventInfo<TEvent>)eventInfo).OnBattleEvent += callback;
            }
        }

        public void TriggerEvent<TEvent>(TEvent battleEvent) where TEvent : BattleEvent
        {
            var eventType = typeof(TEvent);
            if (_typeToEventInfoMap.TryGetValue(eventType, out var eventInfo))
            {
                // 触发所有订阅者的回调
                (eventInfo as BattleEventInfo<TEvent>)?.Invoke(battleEvent);
            }
        }

        public void RemoveListener<TEvent>(Action<TEvent> callback) where TEvent : BattleEvent
        {
            var eventType = typeof(TEvent);
            if (_typeToEventInfoMap.TryGetValue(eventType, out var eventInfo))
            {
                // 移除指定订阅者的回调
                ((BattleEventInfo<TEvent>)eventInfo).OnBattleEvent -= callback;
            }
        }

        public void Clear()
        {
            _typeToEventInfoMap.Clear();
        }
    }
}
