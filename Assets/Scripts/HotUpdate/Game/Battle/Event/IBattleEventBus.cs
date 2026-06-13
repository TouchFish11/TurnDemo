using System;

namespace HotUpdate.Game.Battle.Event
{
    /// <summary>
    /// 战斗事件总线
    /// </summary>
    public interface IBattleEventBus
    {
        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="callback"></param>
        void AddListener<TEvent>(Action<TEvent> callback) where TEvent : BattleEvent;

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="battleEvent"></param>
        void TriggerEvent<TEvent>(TEvent battleEvent) where TEvent : BattleEvent;

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="callback"></param>
        void RemoveListener<TEvent>(Action<TEvent> callback) where TEvent : BattleEvent;

        /// <summary>
        /// 清理所有事件监听
        /// </summary>
        void Clear();
    }
}
