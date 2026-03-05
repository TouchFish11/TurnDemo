using Core.Components;
using Core.Reflection;
using Core.Service;
using HotUpdate.Battle.Object;
using HotUpdate.Battle.Skill.Conditions;
using HotUpdate.Battle.Skill.Factory;
using HotUpdate.Battle.TargetSelect;
using HotUpdate.Battle.TargetSelect.Strategys;

namespace HotUpdate.Battle.Skill.Component
{
    /// <summary>
    /// ���＼�����
    /// </summary>
    [ComponentId(typeof(MonsterSkillComponent))]
    public class MonsterSkillComponent : SkillComponent
    {
        public override void BattleInit(IBattleEntityObject battleEntity)
        {
            base.BattleInit(battleEntity);
            
            var condition = ServiceLocator.Get<IFactoryManager>().GetFactory<ICastSkillConditionFactory, CastSkillConditionFactory>()
                .GetCastSkillCondition<MonsterDefaultCastSkillCondition>();
            AddCastCondition(condition);
                
            // ��ʼ�������б�
            var strategy = ServiceLocator.Get<IFactoryManager>().GetFactory<ITargetSelectStrategyFactory, TargetSelectStrategyFactory>()
                .GetTargetSelectStrategy<MonsterBaseTargetSelectStrategy>();
            AddTargetSelectStrategy(strategy);
        }
    }
}
