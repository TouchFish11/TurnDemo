using Game.Battle;

using System.Collections.Generic;

/// <summary>
/// 状态添加策略
/// 封装不同技能对应不同状态的添加逻辑
/// </summary>
public interface IStatusAddStrategy
{
    /// <summary>
    /// 添加状态
    /// </summary>
    /// <param name="sourcer"></param>
    /// <param name="targets"></param>
    /// <param name="statusIds"></param>
    void ToAdd(IBattleEntityObject sourcer, List<IBattleEntityObject> targets, params int[] statusIds);
}
