using Core.Components;
using Core.Reflection;
using Core.Service;
using Game.Battle.Objects;
using Game.Battle.Skill.Component;
using Game.Battle.Skill.Condition;
using Game.Battle.TargetSelect;
using GameHotUpdate.Battle.Skill.Conditions;
using GameHotUpdate.Battle.TargetSelect.Strategys;

namespace GameHotUpdate.Skill.Component
{
    /// <summary>
    /// ���＼�����
    /// </summary>
    [ComponentId(typeof(MonsterSkillComponent))]
    public class MonsterSkillComponent : SkillComponent
    {
        public override bool IsRelease { get; protected set; }

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
