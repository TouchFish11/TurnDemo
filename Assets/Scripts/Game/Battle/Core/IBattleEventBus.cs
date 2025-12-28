
using System;

/// <summary>
/// 战斗事件总线接口
/// </summary>
public interface IBattleEventBus
{
    /// <summary>
    /// 添加事件（模块通过此方法注册自己要监听的事件）
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="callback"></param>
    void AddListener<TEvent>(Action<TEvent> callback) where TEvent : Game.Battle.BattleEvent;

    /// <summary>
    /// 触发事件（核心流程通过此方法通知所有订阅者）
    /// </summary>
    /// <param name="battleEvent"></param>
    void TriggerEvent<TEvent>(TEvent battleEvent) where TEvent : Game.Battle.BattleEvent;

    /// <summary>
    /// 移除事件
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="callback"></param>
    void RemoveListener<TEvent>(Action<TEvent> callback) where TEvent : Game.Battle.BattleEvent;

    /// <summary>
    /// 清理总线
    /// </summary>
    void Clear();
}
