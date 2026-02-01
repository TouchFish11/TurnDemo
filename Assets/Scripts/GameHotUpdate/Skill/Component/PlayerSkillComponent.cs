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
    /// ��ҽ�ɫ�������
    /// </summary>
    [ComponentId(typeof(PlayerSkillComponent))]
    public class PlayerSkillComponent : SkillComponent
    {
        public override bool IsRelease { get; protected set; }

        public override void BattleInit(IBattleEntityObject battleEntity)
        {
            base.BattleInit(battleEntity);
            
            var condition = ServiceLocator.Get<IFactoryManager>().GetFactory<ICastSkillConditionFactory, CastSkillConditionFactory>()
                .GetCastSkillCondition<PlayerDefaultCastSkillCondition>();
            AddCastCondition(condition);
                
            var strategy = ServiceLocator.Get<IFactoryManager>().GetFactory<ITargetSelectStrategyFactory, TargetSelectStrategyFactory>()
                .GetTargetSelectStrategy<PlayerBaseTargetSelectStrategy>();
            AddTargetSelectStrategy(strategy);
        }

        /// <summary>
        /// �ͷ��սἼ
        /// ��ʱʹ��
        /// </summary>
        public void ReleaseUltimate()
        {
            IsRelease = true;
        }
    }
}
