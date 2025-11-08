using GameLogic.BattleMoudule.Event;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace GameLogic.BattleMoudule.Core
{
    public abstract class BaseBattleEventInfo
    {

    }

    public class BattleEventInfo<TEvent> : BaseBattleEventInfo where TEvent : BattleEvent
    {
        public event UnityAction<TEvent> OnBattleEvent;

        public void Invoke(TEvent eventInfo)
        {
            OnBattleEvent?.Invoke(eventInfo);
        }
    }


    /// <summary>
    /// 事件总线（核心：负责事件的注册、取消注册、广播）
    /// </summary>
    public class BattleEventCenter
    {
        // 存储“事件类型→战斗事件信息”的映射（订阅者是接收事件的回调方法）
        private static readonly Dictionary<Type, BaseBattleEventInfo> _typeToEventInfoMap = new Dictionary<Type, BaseBattleEventInfo>();

        /// <summary>
        /// 添加事件（模块通过此方法注册自己要监听的事件）
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="callback"></param>
        public static void AddListener<TEvent>(UnityAction<TEvent> callback) where TEvent : BattleEvent
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

        /// <summary>
        /// 触发事件（核心流程通过此方法通知所有订阅者）
        /// </summary>
        /// <param name="battleEvent"></param>
        public static void TriggerEvent<TEvent>(TEvent battleEvent) where TEvent : BattleEvent
        {
            Type eventType = typeof(TEvent);
            if (_typeToEventInfoMap.TryGetValue(eventType, out BaseBattleEventInfo eventInfo))
            {
                // 触发所有订阅者的回调
                (eventInfo as BattleEventInfo<TEvent>).Invoke(battleEvent);
            }
        }
    }
}
