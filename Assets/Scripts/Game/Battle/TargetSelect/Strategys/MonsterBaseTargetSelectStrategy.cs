using Game.Battle;
using UnityEngine;

/// <summary>
/// 怪物基础目标选择策略
/// </summary>
public class MonsterBaseTargetSelectStrategy : ITargetSelectStrategy
{
    public IBattleEntityObject SelectMainTarget(IBattleContext context, IBattleEntityObject caster, SkillInfo skillInfo)
    {
        // 随机选择
        int count = context.GetPlayerObjects().Count;
        int index = Random.Range(0, count);
        return context.GetPlayerObjects()[index];
    }
}
