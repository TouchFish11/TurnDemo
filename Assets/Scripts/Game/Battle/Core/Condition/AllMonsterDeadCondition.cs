using Game.Battle;

/// <summary>
/// 所有怪物死亡条件
/// </summary>
public class AllMonsterDeadCondition : IBattleOverCondition
{
    public bool CheckOver(IBattleContext context)
    {
        return context.GetMonsterObjects().Count == 0;
    }
}
