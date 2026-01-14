using Framework;
using Game.Battle;

/// <summary>
/// 怪物技能组件
/// </summary>
[ComponentId(nameof(MonsterSkillComponent))]
public class MonsterSkillComponent : SkillComponent
{
    public override bool IsRelease { get; protected set; }

    public override void BattleInit(IBattleEntityObject battleEntity)
    {
        base.BattleInit(battleEntity);

        AddCastCondition(IFactory.GetTypeInstance<CastSkillConditionFactory, MonsterDefaultCastSkillCondition>());
        // 初始化策略列表
        AddTargetSelectStrategy(IFactory.GetTypeInstance<TargetSelectStrategyFactory, MonsterBaseTargetSelectStrategy>());
    }
}
