using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Game.Battle
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
        private readonly Dictionary<Type, BaseBattleEventInfo> _typeToEventInfoMap = new Dictionary<Type, BaseBattleEventInfo>();

        public void AddListener<TEvent>(Action<TEvent> callback) where TEvent : BattleEvent
        {
            Type eventType = typeof(TEvent);
            if (!_typeToEventInfoMap.TryGetValue(eventType, out BaseBattleEventInfo eventInfo))
            {
                eventInfo = new BattleEventInfo<TEvent>();
                (eventInfo as BattleEventInfo<TEvent>).OnBattleEvent += callback;
                _typeToEventInfoMap.Add(eventType, eventInfo);
            }
            else
            {
                (eventInfo as BattleEventInfo<TEvent>).OnBattleEvent += callback;
            }
        }

        public void TriggerEvent<TEvent>(TEvent battleEvent) where TEvent : BattleEvent
        {
            Type eventType = typeof(TEvent);
            if (_typeToEventInfoMap.TryGetValue(eventType, out BaseBattleEventInfo eventInfo))
            {
                // 触发所有订阅者的回调
                (eventInfo as BattleEventInfo<TEvent>)?.Invoke(battleEvent);
            }
        }

        public void RemoveListener<TEvent>(Action<TEvent> callback) where TEvent : BattleEvent
        {
            Type eventType = typeof(TEvent);
            if (_typeToEventInfoMap.TryGetValue(eventType, out BaseBattleEventInfo eventInfo))
            {
                // 移除指定订阅者的回调
                (eventInfo as BattleEventInfo<TEvent>).OnBattleEvent -= callback;
            }
        }

        public void Clear()
        {
            _typeToEventInfoMap.Clear();
        }
    }
}
