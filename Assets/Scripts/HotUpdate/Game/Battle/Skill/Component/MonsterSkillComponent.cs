using Core.Components;
using HotUpdate.Base;
using HotUpdate.Game.Battle.Skill.Conditions;
using HotUpdate.Game.Battle.TargetSelect.Strategys;

namespace HotUpdate.Game.Battle.Skill.Component
{
    /// <summary>
    /// 怪物技能组件
    /// </summary>
    [ComponentId(typeof(MonsterSkillComponent))]
    public class MonsterSkillComponent : SkillComponent
    {
        public override void BattleInit(IBattleEntityObject battleEntity)
        {
            base.BattleInit(battleEntity);
            
            var condition = castSkillConditionFactory.GetCastSkillCondition<MonsterDefaultCastSkillCondition>();
            AddCastCondition(condition);
            var strategy = targetSelectStrategyFactory.GetTargetSelectStrategy<MonsterBaseTargetSelectStrategy>();
            AddTargetSelectStrategy(strategy);
        }
    }
}
