using Core.Components;
using Core.DI;
using Core.Reflection;
using HotUpdate.Base;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Skill.Conditions;
using HotUpdate.Game.Battle.Skill.Factory;
using HotUpdate.Game.Battle.TargetSelect;
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
            
            var condition = DIContainer.GetInstance<IFactoryManager>().GetFactory<ICastSkillConditionFactory, CastSkillConditionFactory>()
                .GetCastSkillCondition<MonsterDefaultCastSkillCondition>();
            AddCastCondition(condition);
                
            // ��ʼ�������б�
            var strategy = DIContainer.GetInstance<IFactoryManager>().GetFactory<ITargetSelectStrategyFactory, TargetSelectStrategyFactory>()
                .GetTargetSelectStrategy<MonsterBaseTargetSelectStrategy>();
            AddTargetSelectStrategy(strategy);
        }
    }
}
