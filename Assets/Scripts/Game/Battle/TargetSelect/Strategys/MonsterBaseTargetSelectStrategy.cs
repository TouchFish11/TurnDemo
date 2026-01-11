using Game.Battle;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物基础目标选择策略
/// </summary>
public class MonsterBaseTargetSelectStrategy : ITargetSelectStrategy
{
    public IBattleEntityObject SelectMainTarget(IBattleContext context, IBattleEntityObject caster, SkillInfo skillInfo)
    {
        // 随机选择
        List<IBattleEntityObject> players = new List<IBattleEntityObject>(context.GetPlayerObjects());
        int index = Random.Range(0, players.Count);
        return players[index];
    }
}
