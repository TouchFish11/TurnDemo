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
    /// 玩家技能组件
    /// </summary>
    [ComponentId(typeof(PlayerSkillComponent))]
    public class PlayerSkillComponent : SkillComponent
    {
        public bool IsRelease { get; set; }
        
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
        /// 释放终结技
        /// 点击终结技技能按键后，调用该方法改变标识，触发终结技释放
        /// </summary>
        public void ReleaseUltimate()
        {
            IsRelease = true;
        }
    }
}
