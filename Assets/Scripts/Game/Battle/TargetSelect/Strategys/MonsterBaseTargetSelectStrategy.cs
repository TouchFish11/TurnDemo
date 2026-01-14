using Game.Battle;
using UnityEngine;

/// <summary>
/// 怪物基础目标选择策略
/// </summary>
public class MonsterBaseTargetSelectStrategy : ITargetSelectStrategy
{
    public int Priority => 0;

    public IBattleEntityObject SelectMainTarget(IBattleContext context, IBattleEntityObject caster, SkillInfo skillInfo)
    {
        // 随机选择
        var players = context.GetLivePlayerObjects();
        int count = players.Count;
        int index = Random.Range(0, count);
        return players[index];
    }
}
